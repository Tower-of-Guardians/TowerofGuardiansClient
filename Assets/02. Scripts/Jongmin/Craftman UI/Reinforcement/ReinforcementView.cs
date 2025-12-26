using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ReinforcementView : MonoBehaviour, IReinforcementView
{
    [Header("UI 관련 컴포넌트")]
    [Header("공격력 강화 버튼")]
    [SerializeField] private Button m_atk_upgrade_button;

    [Header("공격력/방어력 강화 버튼")]
    [SerializeField] private Button m_both_upgrade_button;

    [Header("방어력 강화 버튼")]
    [SerializeField] private Button m_def_upgrade_button;

    [Header("취소 버튼")]
    [SerializeField] private Button m_cancel_button;

    [Header("닫기 버튼")]
    [SerializeField] private Button m_close_button;


    private Animator m_animator;
    private ReinforcementPresenter m_presenter;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    public void Inject(ReinforcementPresenter presenter)
    {
        m_presenter = presenter;

        m_atk_upgrade_button.onClick.AddListener(m_presenter.OnClickedAtkUpgrade);
        m_both_upgrade_button.onClick.AddListener(m_presenter.OnClickedBothUpgrade);
        m_def_upgrade_button.onClick.AddListener(m_presenter.OnClickedDefUpgrade);

        m_cancel_button.onClick.AddListener(m_presenter.OnClickedCancel);
        m_close_button.onClick.AddListener(m_presenter.OnClickedClose);
    }

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
    {
        ToggleUI(false);
        ToggleCloseButton(false);
    }

    public void ToggleButtonGroup(bool active)
    {
        m_atk_upgrade_button.interactable = active;
        m_both_upgrade_button.interactable = active;
        m_def_upgrade_button.interactable = active;
    }

    public void ToggleCloseButton(bool active)
        => m_close_button.gameObject.SetActive(active);
    private void ToggleUI(bool active)
        => m_animator.SetBool("Open", active);
}
