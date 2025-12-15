using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ResultView : MonoBehaviour, IResultView
{
    [Header("UI 관련 컴포넌트")]
    [Header("결과 텍스트")]
    [SerializeField] private TMP_Text m_result_label;

    [Header("돌아가기 버튼")]
    [SerializeField] private Button m_back_button;

    [Header("카드 목록")]
    [SerializeField] private CardInventoryUI m_card_inventory_ui;
    [SerializeField] private GameObject m_card_content_container; 

    [Space(30f), Header("에디터 테스트 컴포넌트")]
    [Header("테스트 버튼")]
    [SerializeField] private Button m_test_button;

    private Animator m_animator;
    private ResultPresenter m_presenter;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_test_button.onClick.AddListener(() => {OpenUI(true); });
    }

    public void Inject(ResultPresenter presenter)
    {
        m_presenter = presenter;

        m_test_button.onClick.AddListener(() => {m_presenter.OpenUI(new ResultData(BattleResultType.Victory, 78, 46));});
        m_back_button.onClick.AddListener(m_presenter.CloseUI);
    }

    public void OpenUI(bool is_victory)
    {
        m_result_label.text = is_victory ? "전투 승리!"
                                         : "전투 패배!";
        ToggleUI(true);
        
        // 카드 목록 표시
        DisplayCards();
    }

    /// 카드 목록 표시
    private void DisplayCards()
    {
        if (m_card_inventory_ui == null)
        {
            Debug.LogWarning("ResultView: CardInventoryUI가 설정되지 않았습니다.");
            return;
        }

        if (m_card_content_container == null)
        {
            Debug.LogWarning("ResultView: 카드 컨테이너가 설정되지 않았습니다.");
            return;
        }

        if (DataCenter.Instance != null)
        {
            DataCenter.Instance.SortUserCards(SortType.Grade);
        }

        m_card_inventory_ui.DisplayCardsInContainer(m_card_content_container);
    }

    public void CloseUI()
        => ToggleUI(false);

    private void ToggleUI(bool active)
        => m_animator.SetBool("Open", active);
}
