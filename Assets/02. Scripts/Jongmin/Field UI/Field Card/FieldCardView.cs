using System;
using UnityEngine.EventSystems;
using UnityEngine;

public class FieldCardView : CardView, IFieldCardView
{
    [Space(30f), Header("추가 UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    public void OnBeginDrag(PointerEventData eventData)
        => OnBeginDragAction?.Invoke();

    public void OnDrag(PointerEventData eventData)
    {
        ToggleRaycast(false);
        OnDragAction?.Invoke(eventData.position);
        ToggleRaycast(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ToggleRaycast(false);
        OnEndDragAction?.Invoke();
        ToggleRaycast(true);
    }

    private void ToggleRaycast(bool active)
        => m_canvas_group.blocksRaycasts = active;
}