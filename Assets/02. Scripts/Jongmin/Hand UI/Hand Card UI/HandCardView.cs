using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class HandCardView : CardView, IHandCardView
{
    [Space(30f), Header("추가 UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    public event Action OnPointerEnterAction;
    public event Action OnPointerExitAction;
    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;
    public event Action OnPointerClickAction;

    public override void Return()
    {
        transform.DOKill();
        base.Return();
    }

    private void ToggleRaycast(bool active)
        => m_canvas_group.blocksRaycasts = active;

#region Events
    public void OnPointerEnter(PointerEventData eventData)
        => OnPointerEnterAction?.Invoke();

    public void OnPointerExit(PointerEventData eventData)
        => OnPointerExitAction?.Invoke();

    public void OnBeginDrag(PointerEventData eventData)
        => OnBeginDragAction?.Invoke();
        
    public void OnDrag(PointerEventData eventData)
        => OnDragAction?.Invoke(eventData.position);

    public void OnEndDrag(PointerEventData eventData)
    {
        ToggleRaycast(false);
        OnEndDragAction?.Invoke();
        ToggleRaycast(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            OnPointerClickAction?.Invoke();
    }
    #endregion Events
}
