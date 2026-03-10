public class DeckStatusCardPresenter : CardPresenter
{
    private readonly IDeckStatusCardView m_view;

    public DeckStatusCardPresenter(IDeckStatusCardView view,
                                   BattleCardData card_data)
    {
        m_view = view;
        BattleCardData = card_data;

        m_view.UpdateUI(card_data.data);
    }
}
