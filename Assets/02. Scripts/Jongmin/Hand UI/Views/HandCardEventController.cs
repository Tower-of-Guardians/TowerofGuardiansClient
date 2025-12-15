using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardEventController : MonoBehaviour, IDropHandler
{
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    private HandUIDesigner m_designer;
    private HandPresenter m_presenter;
    private HandCardContainer m_container;
    private HandCardLayoutController m_layout_controller;
    private CardInfoUI m_card_info_ui;
    private TurnManager m_turn_manager;

    private Dictionary<IHandCardView, HandCardEventBundle> m_event_dict = new();

    public void Inject(HandUIDesigner designer,
                       HandPresenter presenter,
                       HandCardContainer container,
                       HandCardLayoutController layout_controller,
                       CardInfoUI card_info_ui,
                       TurnManager turn_manager)
    {
        m_designer = designer;
        m_presenter = presenter;
        m_container = container;
        m_layout_controller = layout_controller;
        m_card_info_ui = card_info_ui;
        m_turn_manager = turn_manager;
    }

    public void Subscribe(IHandCardView card_view)
    {
        var new_bundle = new HandCardEventBundle
        {
            OnPointerEnter =    ()          => { OnPointerEnterInCard(card_view); },
            OnPointerExit =     ()          => { OnPointerExitFromCard(); },
            OnBeginDrag =       ()          => { OnBeginDragCard(); },
            OnDrag =            (position)  => { OnDragCard(position); },
            OnEndDrag =         ()          => { OnEndDragCard(); },
            OnPointerClick =    ()          => { OnPointerClickCard(); }
        };

        m_event_dict[card_view] = new_bundle;

        card_view.OnPointerEnterAction += new_bundle.OnPointerEnter;
        card_view.OnPointerExitAction += new_bundle.OnPointerExit;
        card_view.OnBeginDragAction += new_bundle.OnBeginDrag;
        card_view.OnDragAction += new_bundle.OnDrag;
        card_view.OnEndDragAction += new_bundle.OnEndDrag;
        card_view.OnPointerClickAction += new_bundle.OnPointerClick;
    }

    public void Unsubscribe(IHandCardView card_view)
    {
        if(m_event_dict.TryGetValue(card_view, out var bundle))
        {
            card_view.OnPointerEnterAction -= bundle.OnPointerEnter;
            card_view.OnPointerExitAction -= bundle.OnPointerExit;
            card_view.OnBeginDragAction -= bundle.OnBeginDrag;
            card_view.OnDragAction -= bundle.OnDrag;
            card_view.OnEndDragAction -= bundle.OnEndDrag;    
            card_view.OnPointerClickAction -= bundle.OnPointerClick;        
        }
    }

    private void OnPointerEnterInCard(IHandCardView card_view)
    {
        m_presenter.HoverCard = card_view;
        m_layout_controller.UpdateLayout();
    }

    private void OnPointerExitFromCard()
    {
        m_presenter.HoverCard = null;
        m_layout_controller.UpdateLayout();
    }

    private void OnBeginDragCard()
    {
        (m_presenter.HoverCard as HandCardView).transform.DOKill();

        if(m_turn_manager.CanAction())
        {
            m_presenter.ToggleFieldPreview(true);
            CalculatePreviewPosition();
        }
    }

    private void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as HandCardView;
        target_card.transform.position = position;

        var hand_card = GetIHandCardView();
        if(hand_card != null)
        {
            if(m_container.IsExist(hand_card))
            {
                SwapInSameField(hand_card, position);
                CalculatePreviewPosition();
            }
        }
        else
        {
            var drop_hit = CheckField(out var _);
            var drop_handler = drop_hit?.gameObject.GetComponent<IDropHandler>();

            if(drop_handler != null)
                target_card.transform.DOScale(0.75f, m_designer.AnimeSPD);
            else
                target_card.transform.DOScale(m_designer.Scale, m_designer.AnimeSPD);
        }
    }

    private void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);
            
        m_presenter.ToggleFieldPreview(false);

        m_preview_object.SetActive(false);
        CommitChange();
        m_layout_controller.UpdateLayout();
    }

    private void OnPointerClickCard()
    {
        m_card_info_ui.ShowCardInfo(m_presenter.GetCardData(m_presenter.HoverCard).data);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped_object = eventData.pointerDrag;
        if(dropped_object != null)
        {
            var throw_card_view = dropped_object.GetComponent<IThrowCardView>();
            if(throw_card_view != null)
                m_presenter.OnDroped(throw_card_view);

            var field_card_view = dropped_object.GetComponent<IFieldCardView>();
            if(field_card_view != null)
                m_presenter.OnDroped(field_card_view);
        }
    }

    public void CommitChange()
    {
        var views = m_container.Cards;

        for (int i = 0; i < views.Count; i++)
        {
            var presenter = m_container.GetPresenter(views[i]);
            var card_id = presenter.CardData.data.id;

            if (i < GameData.Instance.handDeck.Count)
                GameData.Instance.handDeck[i] = card_id;
            else
                GameData.Instance.handDeck.Add(card_id);
        }

        while (GameData.Instance.handDeck.Count > views.Count)
            GameData.Instance.handDeck.RemoveAt(GameData.Instance.handDeck.Count - 1);
    }

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as HandCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var card_hit = hit.gameObject.GetComponent<IHandCardView>();
            if(card_hit != null && m_presenter.HoverCard != card_hit)
                return hit;

            var field_handler = hit.gameObject.GetComponent<FieldCardEventController>();
            if(field_handler != null)
                return hit;

            var throw_handler = hit.gameObject.GetComponent<ThrowCardEventController>();
            if(throw_handler != null)
                return hit; 
        } 

        return null;
    }

    private IHandCardView GetIHandCardView()
    {
        var hit = CheckField(out _);
        return hit?.gameObject.GetComponent<IHandCardView>();
    }

    private void SwapInSameField(IHandCardView hand_card, Vector2 position)
    {
        var target_card = m_presenter.HoverCard;
        var concrete_card = hand_card as HandCardView;

        if(m_container.IsPriority(target_card, hand_card))
        {
            if(position.x >= concrete_card.transform.position.x)
            {
                m_container.Swap(target_card, hand_card);
                m_layout_controller.UpdateLayout(true);
            }
        }
        else
        {
            if(position.x < concrete_card.transform.position.x)
            {
                m_container.Swap(target_card, hand_card);
                m_layout_controller.UpdateLayout(true);
            }
        }        
    }

    private void CalculatePreviewPosition()
    {
        var layout_data = CardLayoutCalculator.CalculatedHandCardTransform(m_container.GetIndex(m_presenter.HoverCard),
                                                                           m_container.Cards.Count,
                                                                           m_designer.Radius,
                                                                           m_designer.Angle,
                                                                           m_designer.Depth);      
        m_preview_object.SetActive(true);

        var preview_rt = m_preview_object.transform as RectTransform;
        preview_rt.anchoredPosition = layout_data.Position;
        preview_rt.rotation = Quaternion.Euler(layout_data.Rotation);
        preview_rt.localScale = layout_data.Scale;
    }
}
