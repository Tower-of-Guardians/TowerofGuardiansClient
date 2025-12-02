using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ThrowCardEventController : MonoBehaviour, IDropHandler
{
    [Header("의존성 목록")]
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("카드 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    private IThrowView m_view;
    private ThrowPresenter m_presenter;
    private ThrowUIDesigner m_designer;
    private ThrowCardContainer m_container;
    private ThrowCardLayoutController m_layout_controller;

    private Dictionary<IThrowCardView, ThrowCardEventBundle> m_event_dict = new();

    public void Inject(IThrowView view,
                       ThrowPresenter presenter,
                       ThrowUIDesigner designer,
                       ThrowCardContainer container,
                       ThrowCardLayoutController layout_controller)
    {
        m_view = view;
        m_presenter = presenter;
        m_designer = designer;
        m_container = container;
        m_layout_controller = layout_controller;
    }

    public void Subscribe(IThrowCardView card_view)
    {
        var new_bundle = new ThrowCardEventBundle
        {
            OnBeginDrag =   ()          => OnBeginDragCard(card_view),
            OnDrag =        (position)  => OnDragCard(position),
            OnEndDrag =     ()          => OnEndDragCard(),
        };

        m_event_dict[card_view] = new_bundle;

        card_view.OnBeginDragAction += new_bundle.OnBeginDrag;
        card_view.OnDragAction += new_bundle.OnDrag;
        card_view.OnEndDragAction += new_bundle.OnEndDrag;
    }

    public void Unsubscribe(IThrowCardView card_view)
    {
        if(m_event_dict.TryGetValue(card_view, out var bundle))
        {
            card_view.OnBeginDragAction -= bundle.OnBeginDrag;
            card_view.OnDragAction -= bundle.OnDrag;
            card_view.OnEndDragAction -= bundle.OnEndDrag;

            m_event_dict.Remove(card_view);            
        }
    }

    public void OnBeginDragCard(IThrowCardView card_view)
    {
        m_presenter.HoverCard = card_view;

        SetCardParentToCanvas();
        CalculatePreviewPosition();
    }

    public void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        MoveCardToMousePosition(position);

        var throw_card = GetIThrowCardView();
        if(throw_card == null)
            return;

        if(throw_card != null)
        {
            SwapInSameField(throw_card, position);
            CalculatePreviewPosition();
        }
    }

    public void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        SetCardParentToRoot();
        CheckAndInvokeDropHandler();

        m_presenter.HoverCard = null;

        m_layout_controller.UpdateLayout(false, true, true);
        m_preview_object.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped_object = eventData.pointerDrag;
        if(dropped_object != null)
        {
            var card_view = dropped_object.GetComponent<IHandCardView>();
            if(card_view != null)
                m_presenter.OnDroped(card_view);
        }
    }

#region Helper Methods
    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as ThrowCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var card_view = hit.gameObject.GetComponent<IThrowCardView>();
            if(card_view != null && card_view != m_presenter.HoverCard)
                return hit;

            var drop_handler = hit.gameObject.GetComponent<IDropHandler>();
            if(drop_handler != null)
                return hit; 
        } 

        return null;
    }

    private void CalculatePreviewPosition()
    {
        m_preview_object.SetActive(true);
        (m_preview_object.transform as RectTransform).anchoredPosition
            = CardLayoutCalculator.CalculatedThrowCardPosition(m_container.GetIndex(m_presenter.HoverCard),
                                                               m_container.Cards.Count,
                                                               m_designer.Space);
    }

    private void SetCardParentToCanvas()
    {
        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.DOKill();
        target_card.transform.SetParent(m_canvas.transform, false);
    }

    private void MoveCardToMousePosition(Vector2 position)
    {
        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.position = position;
    }

    private IThrowCardView GetIThrowCardView()
    {
        var hit = CheckField(out var _);
        return hit?.gameObject.GetComponent<IThrowCardView>();        
    }

    private void SwapInSameField(IThrowCardView throw_card, Vector2 position)
    {
        var target_card = m_presenter.HoverCard;
        var concrete_card = throw_card as ThrowCardView;

        if(m_container.IsPriority(target_card, throw_card))
        {
            if(position.x >= concrete_card.transform.position.x)
            {
                m_container.Swap(target_card, throw_card);
                m_layout_controller.UpdateLayout(false);
            }
        }
        else
        {
            if(position.x < concrete_card.transform.position.x)
            {
                m_container.Swap(target_card, throw_card);
                m_layout_controller.UpdateLayout(false);
            }
        }        
    }

    private void SetCardParentToRoot()
    {
        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.SetParent(m_slot_root, false);
    }

    private void CheckAndInvokeDropHandler()
    {
        var hand_hit = CheckField(out var pointer_data);
        var drop_handler = hand_hit?.gameObject.GetComponent<HandView>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hand_hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);
    }
#endregion Helper Methods
}
