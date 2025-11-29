using UnityEngine;

public class BattleShopCardFactory : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("슬롯의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("전투 상점 카드 프리펩")]
    [SerializeField] private GameObject m_card_prefab;

    public IBattleShopSlotView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false);

        return card_obj.GetComponent<IBattleShopSlotView>();
    }

    public void RemoveCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IBattleShopSlotView>();

        foreach(var card_view in card_views)
        {
            var target_card = card_view as BattleShopSlotView;
            ObjectPoolManager.Instance.Return(target_card.gameObject);
        }
    }
}