using System;
using UnityEngine.EventSystems;
using UnityEngine;

public interface IThrowCardView : ICardView,
                                  IBeginDragHandler,
                                  IDragHandler,
                                  IEndDragHandler
{
    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;
}