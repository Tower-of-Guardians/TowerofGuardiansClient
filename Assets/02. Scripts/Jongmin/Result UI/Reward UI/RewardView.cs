using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RewardView : MonoBehaviour, IRewardView
{
    [Header("UI 관련 컴포넌트")]
    [Header("골드 텍스트")]
    [SerializeField] private TMP_Text m_gold_label;

    [Header("경험치 텍스트")]
    [SerializeField] private TMP_Text m_exp_label;

    [Header("레벨업 애니메이터")]
    [SerializeField] private Animator m_level_up_animator;

    private Animator m_animator;
    private RewardPresenter m_presenter;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Inject(RewardPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void OpenUI(int gold, int exp, bool is_level_up)
    {
        m_gold_label.text = $"· 골드 +{gold}";
        m_exp_label.text = $"· 경험치 +{exp}";

        ToggleActive(true);
    }

    public void CloseUI()
    {
        ToggleActive(false);
    }

    private void ToggleActive(bool active)
    {
        m_animator.SetBool("Open", active);
    }

    public void CallbackToLevelUpAnimator(int active_flag)
    {
        if(!m_presenter.LevelUp)
            return;

        m_level_up_animator.SetBool("Open", active_flag == 1);
    }
}