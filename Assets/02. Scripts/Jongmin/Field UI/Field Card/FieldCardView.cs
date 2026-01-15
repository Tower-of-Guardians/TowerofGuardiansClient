using System;
using UnityEngine.EventSystems;
using UnityEngine;

public class FieldCardView : CardView, IFieldCardView
{
    [Space(30f), Header("추가 UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("공격 잠금 이미지")]
    [SerializeField] private GameObject m_atk_lock_image;

    [Header("방어 잠금 이미지")]
    [SerializeField] private GameObject m_def_lock_image;

    public event Action OnBeginDragAction;
    public event Action<Vector2> OnDragAction;
    public event Action OnEndDragAction;

    public void OnBeginDrag(PointerEventData eventData)
        => OnBeginDragAction?.Invoke();

    public void OnDrag(PointerEventData eventData)
    {
        ToggleRaycast(false);
        OnDragAction?.Invoke(eventData.position);
        ToggleRaycast(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ToggleRaycast(false);
        OnEndDragAction?.Invoke();
        ToggleRaycast(true);
    }

    private void ToggleRaycast(bool active)
        => m_canvas_group.blocksRaycasts = active;

    public void InitUI(CardData card_data, bool is_atk)
    {
        InitUI(card_data);

        m_atk_lock_image.SetActive(!is_atk);
        m_def_lock_image.SetActive(is_atk);
    }

    public void ToggleLock()
    {
        m_atk_lock_image.SetActive(!m_atk_lock_image.activeInHierarchy);
        m_def_lock_image.SetActive(!m_def_lock_image.activeInHierarchy);
    }
}