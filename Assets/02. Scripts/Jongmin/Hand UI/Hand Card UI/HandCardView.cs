using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class HandCardView : MonoBehaviour, IHandCardView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("카드 테두리 이미지")]
    [SerializeField] private Image m_outline_image;

    [Header("카드 이미지")]
    [SerializeField] private Image m_card_image;

    [Header("카드 이름 텍스트")]
    [SerializeField] private TMP_Text m_name_label;

    [Header("카드 설명 텍스트")]
    [SerializeField] private TMP_Text m_description_label;

    private HandCardPresenter m_presenter;

    public event Action OnPointerEnterAction;
    public event Action OnPointerExitAction;
    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    public void Inject(HandCardPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void InitUI(CardData card_data)
    {
        // TODO: 카드 테두리 이미지 설정
        // TODO: 카드 초상화 이미지 설정

        m_name_label.text = card_data.Name;
        m_description_label.text = card_data.Description;
    }

    public void ToggleRaycast(bool active)
        => m_canvas_group.blocksRaycasts = active;

    public void Return()
    {
        transform.DOKill();
        ObjectPoolManager.Instance.Return(gameObject);
    }

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
#endregion Events
}
