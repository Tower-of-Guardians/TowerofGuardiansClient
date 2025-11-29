public class BattleShopPresenter
{
    private readonly IBattleShopView m_view;
    private readonly BattleShopCardFactory m_factory;
    // TOOD: 카드에 필요한 의존성 변수

    public BattleShopPresenter(IBattleShopView view,
                               BattleShopCardFactory factory)
    {
        m_view = view;
        m_factory = factory;
        
        m_view.Inject(this);
    }

    public void OpenUI()
    {
        // TODO: 레벨에 따른 확률 정보 로드
        m_view.OpenUI();
    }

    public void CloseUI()
        => m_view.CloseUI();

    public void InstantiateCard()
    {
        var slot_view = m_factory.InstantiateCardView();
        var slot_presenter = new BattleShopSlotPresenter(slot_view);
    }

    public void RemoveCards()
        => m_factory.RemoveCards();
}
