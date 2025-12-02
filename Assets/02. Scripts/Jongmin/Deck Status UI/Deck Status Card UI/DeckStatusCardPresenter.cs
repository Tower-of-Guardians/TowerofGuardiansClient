public class DeckStatusCardPresenter : CardPresenter
{
    private readonly IDeckStatusCardView m_view;

    public DeckStatusCardPresenter(IDeckStatusCardView view,
                                   BattleCardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(card_data.data);
    }

    public override void Return()
        => m_view.Return();
}
