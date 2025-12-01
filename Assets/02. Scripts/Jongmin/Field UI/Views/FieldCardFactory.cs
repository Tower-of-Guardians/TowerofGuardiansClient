using UnityEngine;

public class FieldCardFactory : MonoBehaviour, IFieldCardFactory
{
    [Header("의존성 목록")]
    [Header("카드의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("필드 카드 프리펩")]
    [SerializeField] private GameObject m_field_card_prefab;

    private FieldCardEventController m_event_controller;

    public void Inject(FieldCardEventController event_controller)
    {
        m_event_controller = event_controller;
    }

    public IFieldCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_field_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false);
        card_obj.transform.localScale = Vector3.one;

        var card_view = card_obj.GetComponent<IFieldCardView>();
        m_event_controller.Subscribe(card_view);

        return card_view;
    }

    public void ReturnCard(IFieldCardView card_view)
    {
        var target_card = card_view as FieldCardView;
        
        m_event_controller.Unsubscribe(card_view);
        ObjectPoolManager.Instance.Return(target_card.gameObject);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IFieldCardView>();
        foreach(var card_view in card_views)
        {
            m_event_controller.Unsubscribe(card_view);
            
            var card_obj = (card_view as FieldCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }              
    }
}
