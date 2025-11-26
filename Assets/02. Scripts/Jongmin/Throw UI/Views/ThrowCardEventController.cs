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
    private ThrowCardLayoutController m_layout_controller;

    public void Inject(IThrowView view,
                       ThrowPresenter presenter,
                       ThrowCardLayoutController layout_controller)
    {
        m_view = view;
        m_presenter = presenter;
        m_layout_controller = layout_controller;
    }

    public void Subscribe(IThrowCardView card_view)
    {
        card_view.OnBeginDragAction += () => OnBeginDragCard(card_view);
        card_view.OnDragAction += (pos) => OnDragCard(pos);
        card_view.OnEndDragAction += () => OnEndDragCard();
    }

    public void OnBeginDragCard(IThrowCardView card_view)
    {
        m_presenter.HoverCard = card_view;
        
        var target_card = card_view as ThrowCardView;
        target_card.transform.DOKill();
        target_card.transform.SetParent(m_canvas.transform, false);
    }

    public void OnDragCard(Vector2 position)
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.position = position;
    }

    public void OnEndDragCard()
    {
        if(m_presenter.HoverCard == null)
            return;

        var target_card = m_presenter.HoverCard as ThrowCardView;
        target_card.transform.SetParent(m_slot_root, false);

        var hit = CheckField(out var pointer_data);
        var drop_handler = hit?.gameObject.GetComponent<IDropHandler>();
        if(drop_handler != null)
            ExecuteEvents.Execute(hit?.gameObject, pointer_data, ExecuteEvents.dropHandler);

        m_layout_controller.UpdateLayout(m_slot_root, false, true);
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

        m_view.ToggleManual(false);
        m_preview_object.transform.SetAsFirstSibling();
    }

    private RaycastResult? CheckField(out PointerEventData pointer_data)
    {
        pointer_data = new PointerEventData(EventSystem.current);
        pointer_data.position = Input.mousePosition;
        pointer_data.pointerDrag = (m_presenter.HoverCard as ThrowCardView).gameObject;

        var ray_hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer_data, ray_hits);

        foreach(var hit in ray_hits)
        {
            var drop_handler = hit.gameObject.GetComponent<IDropHandler>();
            if(drop_handler != null)
                return hit; 
        } 

        return null;
    }
}
