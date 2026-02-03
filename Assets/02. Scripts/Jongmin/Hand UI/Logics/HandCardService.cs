public class HandCardService
{
    private readonly IHandCardFactory m_factory;
    private readonly HandCardContainer m_container;
    private readonly HandCardLayoutController m_layout_controller;

    public HandCardService(IHandCardFactory factory,
                           HandCardContainer container,
                           HandCardLayoutController layout_controller)
    {
        m_factory = factory;
        m_container = container;
        m_layout_controller = layout_controller;
    }

    public void Add(BattleCardData card_data)
    {
        var view = m_factory.InstantiateCardView();
        var presenter = new HandCardPresenter(view, 
                                              card_data);
        
        m_container.Add(view, presenter);
        m_layout_controller.UpdateLayout();
    }

    public void Remove(IHandCardView card_view, bool layout_update)
    {
        if(m_container.Dict.TryGetValue(card_view, out _))
        {
            m_container.Remove(card_view);
            m_factory.ReturnCard(card_view);

            if(layout_update)
                m_layout_controller.UpdateLayout();
        }
    }

    public void RemoveAll()
    {
        m_container.Clear();
        m_factory.ReturnCards();
    }
}
