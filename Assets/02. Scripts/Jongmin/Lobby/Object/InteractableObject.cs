using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableObject : MonoBehaviour
{
    public event Action OnMouseEnterAction;
    public event Action OnMouseExitAction;
    public event Action OnMouseDownAction;
    public event Action OnMouseUpAction;

    protected virtual void OnMouseEnter()
    {
        if(IsPointerOverUI())
            return;

        OnMouseEnterAction?.Invoke();
    }

    protected virtual void OnMouseExit()
        => OnMouseExitAction?.Invoke();

    protected virtual void OnMouseDown()
    {
        if(IsPointerOverUI())
            return;

        OnMouseDownAction?.Invoke();
    }

    protected virtual void OnMouseUp()
    {
        if(IsPointerOverUI())
            return;
            
        OnMouseUpAction?.Invoke();
    }

    private bool IsPointerOverUI()
    {
        if(EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }
}
