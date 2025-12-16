using UnityEngine;

public class CardInventoryFactory : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("카드 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("인벤토리 카드")]
    [SerializeField] private GameObject m_card_prefab;

    public IInventoryCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false);
        card_obj.transform.localScale = Vector3.one;

        var card_view = card_obj.GetComponent<IInventoryCardView>();

        return card_view;
    }

    public void ReturnCard(IInventoryCardView card_view)
    {
        var target_card = card_view as InventoryCardView;
        
        ObjectPoolManager.Instance.Return(target_card.gameObject);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IInventoryCardView>();
        foreach(var card_view in card_views)
        {  
            var card_obj = (card_view as InventoryCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }              
    }    
}
