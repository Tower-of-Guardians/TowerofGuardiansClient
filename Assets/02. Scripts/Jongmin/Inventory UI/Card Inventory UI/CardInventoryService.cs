public class CardInventoryService
{
    private readonly CardInventoryContainer m_container;
    private readonly CardInventoryFactory m_factory;
    private readonly ICardBehavior m_behavior;
    private ICardSelectionRequester m_selection_requester;
    private ICardSelectionController m_selection_controller;

    public CardInventoryService(CardInventoryContainer container,
                                CardInventoryFactory factory,
                                ICardBehavior behavior)
    {
        m_container = container;
        m_factory = factory;
        m_behavior = behavior;
    }

    public void SetSelectionRequester(ICardSelectionRequester selection_requester)
        => m_selection_requester = selection_requester;

    public void SetSelectionController(ICardSelectionController selection_controller)
        => m_selection_controller = selection_controller;

    public void Add(CardData card_data)
    {
        var view = m_factory.InstantiateCardView();
        var presenter = new InventoryCardPresenter(view, 
                                                   card_data, 
                                                   m_behavior, 
                                                   m_selection_requester,
                                                   m_selection_controller);

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
