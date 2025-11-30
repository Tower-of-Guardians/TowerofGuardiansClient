public class DeckStatusCardPresenter : CardPresenter
{
    private readonly IDeckStatusCardView m_view;

    public DeckStatusCardPresenter(IDeckStatusCardView view,
                                   CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;
    }

    public override void Return()
        => m_view.Return();
}
