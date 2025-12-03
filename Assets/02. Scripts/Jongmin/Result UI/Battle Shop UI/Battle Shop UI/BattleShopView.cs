using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BattleShopView : MonoBehaviour, IBattleShopView
{
    [Header("UI 관련 컴포넌트")]
    [Header("확률 텍스트")]
    [SerializeField] private TMP_Text m_card_rate_label;

    [Header("새로고침 버튼")]
    [SerializeField] private Button m_refresh_button;

    private Animator m_animator;
    private BattleShopPresenter m_presenter;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_refresh_button.onClick.AddListener(() => { m_refresh_button.interactable = false;
                                                     m_animator.SetBool("Open", false);
                                                     m_animator.SetTrigger("Refresh");} );
    }

    public void Inject(BattleShopPresenter presenter)
        => m_presenter = presenter;

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
    {
        ToggleUI(false);
        m_animator.SetTrigger("Close");
    }

    public void UpdateRate(string rate_string)
        => m_card_rate_label.text = rate_string;

    private void ToggleUI(bool active)
        => m_animator.SetBool("Open", active);

    public void CallbackToInstantiateCard()
        => m_presenter.InstantiateCard();

    public void CallbackToDestroyCard()
        => m_presenter.RemoveCards();

    public void CallbackToRefresh()
        => m_presenter.Refresh();
}