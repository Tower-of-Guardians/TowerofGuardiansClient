
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IFieldCardView : ICardView,
                                  IBeginDragHandler,
                                  IDragHandler,
                                  IEndDragHandler
{
    event Action OnBeginDragAction;
    event Action<Vector2> OnDragAction;
    event Action OnEndDragAction;
}