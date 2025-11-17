using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class DeckStatusView : MonoBehaviour, IDeckStatusView
{
    [Header("UI 관련 컴포넌트")]
    [Header("타이틀 이름 텍스트")]
    [SerializeField] private TMP_Text m_title_name_label;

    [Header("슬롯의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("나가기 버튼")]
    [SerializeField] private Button m_exit_button;

    [Header("후보 카드 덱 버튼")]
    [SerializeField] private Button m_draw_card_button;

    [Header("후보 카드 덱 버튼 텍스트")]
    [SerializeField] private TMP_Text m_draw_card_label;

    [Header("교체 카드 덱 버튼")]
    [SerializeField] private Button m_throw_card_button;

    [Header("교체 카드 덱 버튼 텍스트")]
    [SerializeField] private TMP_Text m_throw_card_label;


    [Space(30f), Header("에디터 테스트 옵션")]
    [Header("덱 상태 카드 프리펩")]
    [SerializeField] private GameObject m_deck_status_card_prefab;

    private Animator m_animator;
    private DeckStatusPresenter m_presenter;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    public void Inject(DeckStatusPresenter presenter)
    {
        m_presenter = presenter;

        m_draw_card_button.onClick.AddListener(() => { m_presenter.OpenUI(DeckType.Draw); });
        m_throw_card_button.onClick.AddListener(() => { m_presenter.OpenUI(DeckType.Throw); });
        m_exit_button.onClick.AddListener(m_presenter.CloseUI);
    }

    public void OpenUI()
        => ToggleActive(true);

    public void CloseUI()
        => ToggleActive(false);

    private void ToggleActive(bool active)
        => m_animator.SetBool("Open", active);

    public void UpdateUI(string title_string)
        => m_title_name_label.text = title_string;

    public void UpdateDrawCardCount(int count)
        => m_draw_card_label.text = count.ToString();
    public void UpdateThrowCardCount(int count)
        => m_throw_card_label.text = count.ToString();

    public IDeckStatusCardView InstantiateCardView()
    {
        // TODO: Object Pool을 통한 생성

        var deck_status_card_obj = Instantiate(m_deck_status_card_prefab, m_slot_root);

        return deck_status_card_obj.GetComponent<IDeckStatusCardView>();
    }

    public void ReturnCards()
    {
        // TODO: Object Pool을 통한 제거

        var card_views = m_slot_root.GetComponentsInChildren<IDeckStatusCardView>();
        foreach(var card_view in card_views)
        {
            var card_obj = (card_view as DeckStatusCardView).gameObject;
            Destroy(card_obj);
        }
    }
}
