public class HandCardPresenter : CardPresenter
{
    private readonly IHandCardView m_view;

    public CardData CardData => BattleCardData.data;

    public HandCardPresenter(IHandCardView view,
                             BattleCardData card_data)
    {
        m_view = view;
        BattleCardData = card_data;

        m_view.UpdateUI(card_data.data);
    }
}
