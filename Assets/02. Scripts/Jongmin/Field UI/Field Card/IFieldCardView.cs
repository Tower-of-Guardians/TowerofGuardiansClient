
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IFieldCardView : ICardView,
                                  IBeginDragHandler,
                                  IDragHandler,
                                  IEndDragHandler,
                                  IDropHandler
{
    event Action OnBeginDragAction;
    event Action<Vector2> OnDragAction;
    event Action OnEndDragAction;
    event Action OnDropAction;
}