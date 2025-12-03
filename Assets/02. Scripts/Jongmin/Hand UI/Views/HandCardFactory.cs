using UnityEngine;

public class HandCardFactory : MonoBehaviour, IHandCardFactory
{
    [Header("의존성 목록")]
    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("필드 카드 프리펩")]
    [SerializeField] private GameObject m_field_card_prefab;

    private HandCardEventController m_event_controller;

    public void Inject(HandCardEventController event_controller)
        => m_event_controller = event_controller;

    public IHandCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_field_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false);
        card_obj.transform.localScale = Vector3.one;

        var card_view = card_obj.GetComponent<IHandCardView>();
        m_event_controller.Subscribe(card_view);

        return card_view;
    }

    public void ReturnCard(IHandCardView card_view)
    {
        var target_card = card_view as HandCardView;
        
        m_event_controller.Unsubscribe(card_view);
        ObjectPoolManager.Instance.Return(target_card.gameObject);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IHandCardView>();
        foreach(var card_view in card_views)
        {
            m_event_controller.Unsubscribe(card_view);
            
            var card_obj = (card_view as HandCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }              
    }
}
