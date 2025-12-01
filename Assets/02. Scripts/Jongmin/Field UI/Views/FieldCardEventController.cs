using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldCardEventController : MonoBehaviour, IDropHandler
{
    [Header("의존성 목록")]
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 오브젝트")]
    [SerializeField] private GameObject m_preview_object;

    private IFieldView m_view;
    private FieldPresenter m_presenter;
    private FieldCardLayoutController m_layout_controller;
    private FieldCardContainer m_container;
    private FieldUIDesigner m_designer;

    private readonly Dictionary<IFieldCardView, FieldCardEventBundle> m_event_dict = new();

    public void Inject(IFieldView view,
                       FieldPresenter presenter,
                       FieldCardLayoutController layout_controller,
                       FieldCardContainer container,
                       FieldUIDesigner designer)
    {
        m_view = view;
        m_presenter = presenter;
        m_layout_controller = layout_controller;
        m_container = container;
        m_designer = designer;
    }

    public void Subscribe(IFieldCardView card_view)
    {
        var new_bundle = new FieldCardEventBundle
        {
            OnBeginDrag =   ()          => OnBeginDragCard(card_view),
            OnDrag =        (position)  => OnDragCard(position),
            OnEndDrag =     ()          => OnEndDragCard()
        };

        m_event_dict[card_view] = new_bundle;

        card_view.OnBeginDragAction += new_bundle.OnBeginDrag;
        card_view.OnDragAction += new_bundle.OnDrag;
        card_view.OnEndDragAction += new_bundle.OnEndDrag;        
    }

    public void Unsubscribe(IFieldCardView card_view)
    {
        if(m_event_dict.TryGetValue(card_view, out var bundle))
        {
            card_view.OnBeginDragAction -= bundle.OnBeginDrag;
            card_view.OnDragAction -= bundle.OnDrag;
            card_view.OnEndDragAction -= bundle.OnEndDrag;

            m_event_dict.Remove(card_view);
        }
    }

    public void OnBeginDragCard(IFieldCardView card_view)
    {
        m_presenter.HoverCard = card_view;

        var target_card = m_presenter.HoverCard as FieldCardView;

        m_preview_object.SetActive(true);
        (m_preview_object.transform as RectTransform).anchoredPosition
            = CardLayoutCalculator.CalculatedFieldCardPosition(m_container.GetIndex(m_presenter.HoverCard),
                                                               m_designer.ATKLimit,
                                                               m_designer.Space);

        target_card.transform.DOKill();
        target_card.transform.SetParent(m_canvas.transform, false);
    }

    public void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;
        
        var target_card = m_presenter.HoverCard as FieldCardView;
        target_card.transform.position = position;

        var hit = CheckField(out var pointer_data);

        var field_card = hit?.gameObject.GetComponent<IFieldCardView>();
        if(field_card != null)
        {
            m_preview_object.SetActive(true);

            var concrete_card = field_card as FieldCardView;

            if(m_container.IsPriority(target_card, field_card))
            {
                if(position.x >= concrete_card.transform.position.x)
                {
                    m_container.Swap(target_card, field_card);
                    m_layout_controller.UpdateLayout(false);
                }
            }
            else
            {
                if(position.x < concrete_card.transform.position.x)
                {
                    m_container.Swap(target_card, field_card);
                    m_layout_controller.UpdateLayout(false);
                }
            }

            (m_preview_object.transform as RectTransform).anchoredPosition 
                = CardLayoutCalculator.CalculatedFieldCardPosition(m_container.GetIndex(m_presenter.HoverCard),
                                                                   m_designer.ATKLimit,
                                                                   m_designer.Space);
        }
    }

    public void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as FieldCardView;

        var world_position = target_card.transform.position;
        target_card.transform.SetParent(m_slot_root, false);

        var local_position = target_card.transform.parent.InverseTransformPoint(world_position);
        target_card.transform.localPosition = local_position;

        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<HandView>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);
        
        m_presenter.HoverCard = null;

        m_preview_object.SetActive(false);
        m_layout_controller.UpdateLayout(false);
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

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as FieldCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var card_view = hit.gameObject.GetComponent<IFieldCardView>();
            if(card_view != null && card_view != m_presenter.HoverCard)
                return hit;

            var drop_handler = hit.gameObject.GetComponent<IDropHandler>();
            if(drop_handler != null)
                return hit; 
        } 

        return null;
    }
}
