public class FieldCardService
{
    private readonly IFieldCardFactory m_factory;
    private readonly FieldCardContainer m_container;

    private readonly TurnManager m_turn_manager;

    private readonly FieldUIDesigner m_designer;
    private readonly bool m_is_atk;

    public int Count => m_container.Cards.Count;
    public bool CanAdd => m_container.Cards.Count < (m_is_atk ? m_designer.ATKLimit
                                                              : m_designer.DEFLimit);

    public FieldCardService(IFieldCardFactory factory,
                            FieldCardContainer container,
                            TurnManager turn_manager,
                            FieldUIDesigner designer,
                            bool is_atk)
    {
        m_factory = factory;
        m_container = container;
        m_turn_manager = turn_manager;
        m_designer = designer;
        m_is_atk = is_atk;
    }

    public void Add(CardData card_data)
    {
        var view = m_factory.InstantiateCardView();
        var presenter = new FieldCardPresenter(view, card_data);

        m_container.Add(view, presenter);
        m_turn_manager.UpdateThrowCount(1);
    }

    public void Remove(IFieldCardView card_view)
    {
        if(m_container.Dict.TryGetValue(card_view, out var presenter))
        {
            m_container.Remove(card_view);
            m_factory.ReturnCard(card_view);
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
