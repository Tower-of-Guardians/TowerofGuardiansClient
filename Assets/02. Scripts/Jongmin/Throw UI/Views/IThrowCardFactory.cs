public interface IThrowCardFactory
{
    IThrowCardView InstantiateCardView();
    void ReturnCard(IThrowCardView card_view, BattleCardData card_data);
    void ReturnCards();
}