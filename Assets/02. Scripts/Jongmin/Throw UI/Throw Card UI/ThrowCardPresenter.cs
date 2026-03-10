public class ThrowCardPresenter : CardPresenter
{
    private readonly IThrowCardView m_view;

    public CardData CardData => BattleCardData.data;

    public ThrowCardPresenter(IThrowCardView view, 
                              BattleCardData card_data)
    {
        m_view = view;
        BattleCardData = card_data;

        m_view.UpdateUI(card_data.data);
    }
}