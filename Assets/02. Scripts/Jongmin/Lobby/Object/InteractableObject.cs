using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public event Action OnMouseEnterAction;
    public event Action OnMouseExitAction;
    public event Action OnMouseDownAction;
    public event Action OnMouseUpAction;

    protected virtual void OnMouseEnter()
        => OnMouseEnterAction?.Invoke();

    protected virtual void OnMouseExit()
        => OnMouseExitAction?.Invoke();

    protected virtual void OnMouseDown()
        => OnMouseDownAction?.Invoke();

    protected virtual void OnMouseUp()
        => OnMouseUpAction?.Invoke();
}
