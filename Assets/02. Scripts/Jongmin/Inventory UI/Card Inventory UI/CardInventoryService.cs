public class CardInventoryService
{
    private readonly CardInventoryContainer m_container;
    private readonly CardInventoryFactory m_factory;

    public CardInventoryService(CardInventoryContainer container,
                                CardInventoryFactory factory)
    {
        m_container = container;
        m_factory = factory;
    }

    public void Add(CardData card_data)
    {
        var view = m_factory.InstantiateCardView();
        var presenter = new InventoryCardPresenter(view, card_data);

        m_container.Add(view, presenter);
    }

    public void Remove(IInventoryCardView card_view)
    {
        if(m_container.Dict.TryGetValue(card_view, out var presenter))
        {
            m_container.Remove(card_view);
            m_factory.ReturnCard(card_view);
        }
    }

    public void RemoveAll()
    {   
        m_container.Clear();
        m_factory.ReturnCards();
    }    
}
