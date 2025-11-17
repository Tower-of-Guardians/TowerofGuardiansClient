using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IHandCardView : IPointerEnterHandler, 
                                 IPointerExitHandler,
                                 IBeginDragHandler,
                                 IDragHandler,
                                 IEndDragHandler
{
    public event Action OnPointerEnterAction;
    public event Action OnPointerExitAction;
    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    void Inject(HandCardPresenter presenter);
    void InitUI(CardData card_data);
    void ToggleRaycast(bool active);
    void Return();
}