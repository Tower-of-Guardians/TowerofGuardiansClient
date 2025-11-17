using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BattleShopView : MonoBehaviour, IBattleShopView
{
    [Header("UI 관련 컴포넌트")]
    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("확률 텍스트")]
    [SerializeField] private TMP_Text m_card_rate_label;

    [Header("새로고침 버튼")]
    [SerializeField] private Button m_refresh_button;

    [Space(30f), Header("에디터 테스트 컴포넌트")]
    [Header("전투 상점 슬롯 프리펩")]
    [SerializeField] private GameObject m_slot_prefab;

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
    {
        m_presenter = presenter;
    }

    public IBattleShopSlotView InstantiateSlotView()
    {
        var slot_obj = Instantiate(m_slot_prefab, m_slot_root);

        return slot_obj.GetComponent<IBattleShopSlotView>();
    }

    public void OpenUI()
    {
        ToggleUI(true);
    }

    public void CloseUI()
    {
        ToggleUI(false);
        m_animator.SetTrigger("Close");
    }

    private void ToggleUI(bool active)
    {
        m_animator.SetBool("Open", active);
    }

    public void CallbackToInstantiateCard()
    {
        // TODO: Object Pool을 통한 카드 생성
        
        m_presenter.InstantiateSlot();
    }

    public void CallbackToDestroyCard()
    {
        // TODO: Object Pool을 통한 카드 제거

        for (int i = m_slot_root.childCount - 1; i >= 0; i--)
            Destroy(m_slot_root.GetChild(i).gameObject);
    }
}