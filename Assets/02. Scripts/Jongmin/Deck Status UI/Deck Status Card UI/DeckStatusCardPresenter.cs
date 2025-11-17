public class DeckStatusCardPresenter
{
    private readonly IDeckStatusCardView m_view;
    private readonly CardData m_card_data;

    public DeckStatusCardPresenter(IDeckStatusCardView view,
                                   CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;
    }
}
