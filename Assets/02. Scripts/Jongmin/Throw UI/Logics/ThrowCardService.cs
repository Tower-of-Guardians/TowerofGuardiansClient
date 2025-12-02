public class ThrowCardService
{
    private readonly IThrowView m_view;
    private readonly IThrowCardFactory m_factory;
    private readonly ThrowCardContainer m_container;
    private readonly TurnManager m_turn_manager;
    
    public bool IsThrowed { get; set; }
    public int Count => m_container.Cards.Count;

    public ThrowCardService(IThrowView view,
                            IThrowCardFactory factory,
                            ThrowCardContainer container,
                            TurnManager turn_manager)
    {
        m_view = view;
        m_factory = factory;
        m_container = container;
        m_turn_manager = turn_manager;
    }
    
    public void Add(BattleCardData card_data)
    {
        var view = m_factory.InstantiateCardView();
        var presenter = new ThrowCardPresenter(view, card_data);

        m_container.Add(view, presenter);
        m_turn_manager.UpdateThrowCount(1);
    }

    public void Remove(IThrowCardView card_view)
    {
        if(m_container.Dict.TryGetValue(card_view, out var presenter))
        {
            m_container.Remove(card_view);
            m_factory.ReturnCard(card_view, presenter.CardData);
            m_turn_manager.UpdateThrowCount(-1);
        }
    }

    public void RemoveAll()
    {   
        m_turn_manager.UpdateThrowCount(-Count);
        m_container.Clear();
        m_factory.ReturnCards();
    }
}
