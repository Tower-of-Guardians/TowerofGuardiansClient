using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayoutThrowView : MonoBehaviour, IThrowView
{
    [Header("디자이너")]
    [SerializeField] private ThrowUIDesigner m_designer;

    [Space(30f), Header("UI 관련 컴포넌트")]
    [Header("슬롯의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("프리뷰 카드")]
    [SerializeField] private GameObject m_preview_object;

    [Header("UI 열기 버튼")]
    [SerializeField] private Button m_open_button;

    [Header("UI 닫기 버튼")]
    [SerializeField] private Button m_close_button;

    [Header("버리기 버튼")]
    [SerializeField] private Button m_throw_button;

    [Space(30f), Header("의존성 목록")]
    [Header("교체 애니메이터")]
    [SerializeField] private Animator m_animator;

    [Header("임시 카드 컨트롤러")]
    [SerializeField] private TemporaryCardController m_temp_card_controller;

    [Header("교체 카드 프리펩")]
    [SerializeField] private GameObject m_throw_card_prefab;

    [Header("팝업 알리미 프리펩")]
    [SerializeField] private GameObject m_popup_notice_prefab;

    [Header("카드 패 뷰")]
    [SerializeField] private Transform m_hand_view_transform;

    [Header("교체 카드 버튼")]
    [SerializeField] private Transform m_throw_button_transform;

    private ThrowPresenter m_presenter;

    private void OnDestroy()
        => m_temp_card_controller.OnAnimationEnd -= m_presenter.OnTempCardAnimeEnd;

    public void Inject(ThrowPresenter presenter)
    {
        m_presenter = presenter;

        m_open_button.onClick.AddListener(m_presenter.OnClickedOpenUI);
        m_close_button.onClick.AddListener(m_presenter.OnClickedCloseUI);
        m_throw_button.onClick.AddListener(m_presenter.OnClickedThrowCards);

        m_temp_card_controller.OnAnimationEnd += m_presenter.OnTempCardAnimeEnd;
    }

    public void OpenUI()
        => ToggleAnime(true);

    public void CloseUI()
    {
        ToggleAnime(false);
        
        m_temp_card_controller.PlayAnime(m_presenter.GetCardDatas(),
                                         m_open_button.transform.position,
                                         m_hand_view_transform.position,
                                         0.5f,
                                         200f,
                                         0.75f,
                                         0.1f);
    }

    public void ThrowUI()
    {
        ToggleAnime(false);
        
        m_temp_card_controller.PlayAnime(m_presenter.GetCardDatas(),
                                         m_open_button.transform.position,
                                         m_throw_button_transform.position,
                                         0.3f,
                                         50f,
                                         0.75f,
                                         0.1f);        
    }

    private void ToggleAnime(bool active)
        => m_animator.SetBool("Open", active);

    public void ToggleManual(bool active)
    {
        UpdateLayout(active, !active);
        m_preview_object.SetActive(active);
    }

    public IThrowCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_throw_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false); 

        UpdateLayout(false, false);
        
        return card_obj.GetComponent<IThrowCardView>();
    }

    public void ReturnCard(IThrowCardView card_view, CardData card_data)
    {
        var concrete_card = card_view as ThrowCardView;
        
        m_temp_card_controller.PlayAnime(card_data,
                                         concrete_card.transform.position,
                                         m_hand_view_transform.position,
                                         0.75f,
                                         50f,
                                         0.75f);  

        ObjectPoolManager.Instance.Return(concrete_card.gameObject);

        UpdateLayout(false, true);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        foreach(var card_view in card_views)
        {
            var card_obj = (card_view as ThrowCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }        
    }

    public void PrintNotice(string notice_text)
    {
        var popup_notice_obj = ObjectPoolManager.Instance.Get(m_popup_notice_prefab);

        var popup_notice_ui = popup_notice_obj.GetComponent<IPopupNoticeView>();
        popup_notice_ui.OpenUI(notice_text);
    }

    private void UpdateLayout(bool include_preview, bool is_removing)
    {
        var card_views = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        var card_count = include_preview ? card_views.Length + 1
                                         : card_views.Length;

        if(card_count == 0)
            return;

        var prev_preview_position = card_views.Length > 0 ? ((card_views[^1] as ThrowCardView).transform as RectTransform).anchoredPosition
                                                          : Vector2.zero; 

        var total_width = (card_count - 1) * m_designer.Space;
        for(int i = 0; i < card_views.Length; i++)
        {
            var target_position = CardLayoutCalculator.CalculatedThrowCardPosition(i, card_count, m_designer.Space);
            var concrete_card = (card_views[i] as ThrowCardView).transform as RectTransform;

            if(!is_removing)
                if(card_count - i > 1)
                    concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);
                else
                    concrete_card.anchoredPosition = target_position;
            else
                concrete_card.DOAnchorPos(target_position, m_designer.AnimeDuration).SetEase(Ease.InOutSine);

        }

        if(include_preview)
        {
            var preview_rect = m_preview_object.transform as RectTransform; 
            preview_rect.anchoredPosition = prev_preview_position;
            
            var preview_position = CardLayoutCalculator.CalculatedThrowCardPosition(card_count - 1, card_count, m_designer.Space);
            preview_rect.DOAnchorPos(preview_position, m_designer.AnimeDuration);
        }
    }

#region Events
    public void OnDrop(PointerEventData eventData)
    {
        var dropped_object = eventData.pointerDrag;
        if(dropped_object != null)
        {
            var card_view = dropped_object.GetComponent<IHandCardView>();
            if(card_view != null)
                m_presenter.OnDroped(card_view);
        }

        ToggleManual(false);
        m_preview_object.transform.SetAsFirstSibling();
    }
    #endregion Events
}