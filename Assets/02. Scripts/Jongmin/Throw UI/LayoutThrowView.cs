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

    [Header("교체 → 핸드 이펙터")]
    [SerializeField] private ThrowCardToHandEffector m_throw_card_to_hand_effector;

    [Header("교체 → 교체 이펙터")]
    [SerializeField] private ThrowCardToThrowEffector m_throw_card_to_throw_effector;

    private ThrowPresenter m_presenter;
    private ThrowCardLayoutController m_layout_controller;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(ThrowCardContainer container,
                       ThrowUIDesigner designer,
                       ThrowCardLayoutController layout_controller,
                       ThrowCardEventController event_controller,
                       ThrowCardFactory card_factory)
    {
        m_layout_controller = layout_controller;

        layout_controller.Inject(m_presenter, container);
        event_controller.Inject(this, m_presenter, designer, container, m_layout_controller);
        card_factory.Inject(event_controller, m_layout_controller);
    }

    public void Inject(ThrowPresenter presenter)
    {
        m_presenter = presenter;

        m_open_button.onClick.AddListener(m_presenter.OnClickedOpenUI);
        m_close_button.onClick.AddListener(m_presenter.OnClickedCloseUI);
        m_throw_button.onClick.AddListener(m_presenter.OnClickedThrowCards);
    }

    public void OpenUI()
        => ToggleAnime(true);

    public void CloseUI()
    {
        m_throw_card_to_hand_effector.Execute();
        ToggleAnime(false);
    }

    public void UpdateOpenButton(bool open_active)
        => m_open_button.interactable = open_active;

    public void UpdateThrowButton(bool throw_active)
        => m_throw_button.interactable = throw_active;

    public void ThrowUI()
    {
        m_throw_card_to_throw_effector.Execute();
        ToggleAnime(false);
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