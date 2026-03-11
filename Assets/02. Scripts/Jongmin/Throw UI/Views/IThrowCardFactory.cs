public interface IThrowCardFactory
{
    IDiscardCardUI InstantiateCardView();
    void ReturnCard(IDiscardCardUI card_view, BattleCardData card_data);
    void ReturnCards();
}