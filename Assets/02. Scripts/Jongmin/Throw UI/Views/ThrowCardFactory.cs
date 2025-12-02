using UnityEngine;

public class ThrowCardFactory : MonoBehaviour, IThrowCardFactory
{
    [Header("의존성 목록")]
    [Header("카드 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_root;

    [Header("교체 카드 프리펩")]
    [SerializeField] private GameObject m_throw_card_prefab;

    private ThrowCardEventController m_event_controller;
    private ThrowCardLayoutController m_layout_controller;
    private ThrowAnimeController m_anime_controller;

    public void Inject(ThrowCardEventController event_controller,
                       ThrowCardLayoutController layout_controller,
                       ThrowAnimeController anime_controller)
    {
        m_event_controller = event_controller;
        m_layout_controller = layout_controller;
        m_anime_controller = anime_controller;
    }

    public IThrowCardView InstantiateCardView()
    {
        var card_obj = ObjectPoolManager.Instance.Get(m_throw_card_prefab);
        card_obj.transform.SetParent(m_slot_root, false); 
        card_obj.transform.localScale = Vector3.one;

        var card_view = card_obj.GetComponent<IThrowCardView>();
        m_event_controller.Subscribe(card_view);    
        m_layout_controller.UpdateLayout(false, false);
        
        return card_view;
    }

    public void ReturnCard(IThrowCardView card_view, BattleCardData card_data)
    {
        m_anime_controller.PlayRemove(card_view, card_data);
        m_event_controller.Unsubscribe(card_view);
        m_layout_controller.UpdateLayout(false, true);
    }

    public void ReturnCards()
    {
        var card_views = m_slot_root.GetComponentsInChildren<IThrowCardView>();
        foreach(var card_view in card_views)
        {
            m_event_controller.Unsubscribe(card_view);

            var card_obj = (card_view as ThrowCardView).gameObject;
            ObjectPoolManager.Instance.Return(card_obj);
        }        
    }
}