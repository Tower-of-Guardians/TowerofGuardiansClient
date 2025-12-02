public abstract class CardPresenter
{
    protected BattleCardData m_card_data;

    public BattleCardData CardData => m_card_data;

    public abstract void Return();
}
