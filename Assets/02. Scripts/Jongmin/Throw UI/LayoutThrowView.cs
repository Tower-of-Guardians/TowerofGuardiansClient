using UnityEngine;
using UnityEngine.UI;

public class LayoutThrowView : MonoBehaviour, IThrowView
{
    [Space(30f), Header("UI 관련 컴포넌트")]
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

    private ThrowPresenter m_presenter;

    private ThrowAnimeController m_anime_controller;
    private ThrowCardLayoutController m_layout_controller;

    private void OnDestroy()
    {
        if(m_temp_card_controller != null && m_presenter != null)
            m_temp_card_controller.OnAnimationEnd -= m_presenter.OnTempCardAnimeEnd;

        m_presenter?.Dispose();
    }

    public void Inject(ThrowAnimeController anime_controller,
                       ThrowCardContainer container,
                       ThrowUIDesigner designer,
                       ThrowCardLayoutController layout_controller,
                       ThrowCardEventController event_controller,
                       ThrowCardFactory card_factory)
    {
        m_anime_controller = anime_controller;
        m_layout_controller = layout_controller;

        layout_controller.Inject(m_presenter, container);
        event_controller.Inject(this, m_presenter, designer, container, m_layout_controller);
        card_factory.Inject(event_controller, m_layout_controller, m_anime_controller);
    }

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
        m_anime_controller.PlayRemoveAll(m_presenter.GetCardDatas());
    }

    public void UpdateUI(bool open_active, bool throw_active)
    {
        m_open_button.interactable = open_active;
        m_throw_button.interactable = throw_active;
    }

    public void ThrowUI()
    {
        ToggleAnime(false);
        m_anime_controller.PlayThrowAll(m_presenter.GetCardDatas());
    }

    private void ToggleAnime(bool active)
        => m_animator.SetBool("Open", active);

    public void ToggleManual(bool active)
    {
        m_layout_controller.UpdateLayout(active, active, active);
        m_preview_object.SetActive(active);
        m_preview_object.transform.SetAsFirstSibling();
    }
}