using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowCardView : MonoBehaviour, IThrowCardView
{
    private ThrowCardPresenter m_presenter;

    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    public void Inject(ThrowCardPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void UpdateUI(CardData card_data)
    {
        // TODO: 카드 UI 설정
    }

#region Events
    public void OnBeginDrag(PointerEventData eventData)
        => OnBeginDragAction?.Invoke();

    public void OnDrag(PointerEventData eventData)
        => OnDragAction?.Invoke(eventData.position);

    public void OnEndDrag(PointerEventData eventData)
        => OnEndDragAction?.Invoke();
    #endregion Events
}
