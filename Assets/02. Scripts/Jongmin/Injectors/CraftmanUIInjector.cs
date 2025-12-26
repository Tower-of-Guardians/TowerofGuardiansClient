using UnityEngine;

public class CraftmanUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("대장장이 UI")]
    [SerializeField] private CraftmanView m_craftman_view;

    [Header("인벤토리 UI")]
    [SerializeField] private CraftmanInventoryView m_inventory_view;

    [Header("인벤토리 카드 팩토리")]
    [SerializeField] private CardInventoryFactory m_card_factory;

    [Header("강화 UI")]
    [SerializeField] private ReinforcementView m_reinforcement_view;

    [Header("강화 카드 UI")]
    [SerializeField] private ReinforcementCardView m_card_view;

    [Header("강화 데이터베이스")]
    [SerializeField] private ReinforcementDataBase m_reinforcement_db;

    [Header("알리미 UI")]
    [SerializeField] private Notice m_notice;

    public void Inject()
    {
        InjectCard();
        InjectDB();
        InjectReinforcement();
        InjectInventory();
        InjectCraftman();
    }

    private void InjectCard()
    {
        DIContainer.Register<ReinforcementCardView>(m_card_view);

        var reinforcement_card_presenter = new ReinforcementCardPresenter(m_card_view);
        DIContainer.Register<ReinforcementCardPresenter>(reinforcement_card_presenter);
    }

    private void InjectDB()
        => DIContainer.Register<IReinforcementDataBase>(m_reinforcement_db);

    private void InjectReinforcement()
    {
        DIContainer.Register<IReinforcementView>(m_reinforcement_view);

        var reinforcement_presenter = new ReinforcementPresenter(m_reinforcement_view,
                                                                 DIContainer.Resolve<ReinforcementCardPresenter>(),
                                                                 m_reinforcement_db);
        DIContainer.Register<ReinforcementPresenter>(reinforcement_presenter);
    }

    private void InjectInventory()
    {
        var reinforcement_presenter = DIContainer.Resolve<ReinforcementPresenter>();
        var selection_behavior = new SelectCardBehavior();
        var inventory_presenter = new CraftmanInventoryPresenter(m_inventory_view,
                                                                 m_card_factory,
                                                                 selection_behavior,
                                                                 m_notice,
                                                                 DIContainer.Resolve<CraftmanDialogueBubblePresenter>(),
                                                                 reinforcement_presenter);
        DIContainer.Register<CraftmanInventoryPresenter>(inventory_presenter);

        
    }

    private void InjectCraftman()
    {
        DIContainer.Register<ICraftmanView>(m_craftman_view);

        var reinforcement_presenter = DIContainer.Resolve<ReinforcementPresenter>();
        var inventory_presenter = DIContainer.Resolve<CraftmanInventoryPresenter>();
        var craftman_presenter = new CraftmanPresenter(m_craftman_view,
                                                       inventory_presenter);
        DIContainer.Register<CraftmanPresenter>(craftman_presenter);

        reinforcement_presenter.Inject(craftman_presenter,
                                       inventory_presenter);
    }
}
