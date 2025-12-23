using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleShopSlotView : CardView, IBattleShopSlotView
{
    [Header("UI 관련 컴포넌트")]
    [Header("애니메이터")]
    [SerializeField] private Animator m_animator;

    [Header("카드 가격 텍스트")]
    [SerializeField] private TMP_Text m_cost_label;

    [Header("구매 버튼")]
    [SerializeField] private Button m_purchase_button;

    private BattleShopSlotPresenter m_presenter;

    private void Awake()
    {
        // TODO: 카드 구매 이벤트 등록
    }

    private void OnEnable()
        => m_animator.SetTrigger("Instantiate");

    public void Inject(BattleShopSlotPresenter presenter)
        => m_presenter = presenter;

    public void InitUI(ShopCardData slot_data, bool can_purchase)
    {
        InitUI(slot_data.Card.data);        
        m_cost_label.text = can_purchase ? $"${slot_data.Cost}"
                                         : $"<color=red>${slot_data.Cost}</color>";
        m_purchase_button.interactable = can_purchase;
    }
}
