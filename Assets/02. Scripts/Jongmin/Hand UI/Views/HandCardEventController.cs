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

    private Dictionary<IHandCardView, HandCardEventBundle> m_event_dict = new();

    public void Inject(HandUIDesigner designer,
                       HandPresenter presenter,
                       HandCardContainer container,
                       HandCardLayoutController layout_controller)
    {
        m_designer = designer;
        m_presenter = presenter;
        m_container = container;
        m_layout_controller = layout_controller;
    }

    public void Subscribe(IHandCardView card_view)
    {
        var new_bundle = new HandCardEventBundle
        {
            OnPointerEnter =    ()          => { OnPointerEnterInCard(card_view); },
            OnPointerExit =     ()          => { OnPointerExitFromCard(); },
            OnBeginDrag =       ()          => { OnBeginDragCard(); },
            OnDrag =            (position)  => { OnDragCard(position); },
            OnEndDrag =         ()          => { OnEndDragCard(); }
        };

        m_event_dict[card_view] = new_bundle;

        card_view.OnPointerEnterAction += new_bundle.OnPointerEnter;
        card_view.OnPointerExitAction += new_bundle.OnPointerExit;
        card_view.OnBeginDragAction += new_bundle.OnBeginDrag;
        card_view.OnDragAction += new_bundle.OnDrag;
        card_view.OnEndDragAction += new_bundle.OnEndDrag;
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
        m_presenter.ToggleFieldPreview(true);
    }

    private void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as HandCardView;
        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();

        if(drop_handler != null)
            target_card.transform.DOScale(0.75f, m_designer.AnimeSPD);
        else
            target_card.transform.DOScale(m_designer.Scale, m_designer.AnimeSPD);
        
        target_card.transform.position = position;
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

        m_layout_controller.UpdateLayout();
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

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as HandCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var field_handler = hit.gameObject.GetComponent<FieldCardEventController>();
            if(field_handler != null)
                return hit;

            var throw_handler = hit.gameObject.GetComponent<ThrowCardEventController>();
            if(throw_handler != null)
                return hit; 
        } 

        return null;
    }
}
