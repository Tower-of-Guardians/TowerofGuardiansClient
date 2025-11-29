using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowCardView : CardView, IThrowCardView
{
    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    public void OnBeginDrag(PointerEventData eventData)
        => OnBeginDragAction?.Invoke();

    public void OnDrag(PointerEventData eventData)
        => OnDragAction?.Invoke(eventData.position);

    public void OnEndDrag(PointerEventData eventData)
        => OnEndDragAction?.Invoke();
}
