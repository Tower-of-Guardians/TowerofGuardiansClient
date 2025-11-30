public abstract class CardPresenter
{
    protected CardData m_card_data;

    public CardData CardData => m_card_data;

    public abstract void Return();
}
