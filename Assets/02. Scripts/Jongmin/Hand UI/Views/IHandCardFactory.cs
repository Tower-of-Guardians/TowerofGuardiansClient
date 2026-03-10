public interface IHandCardFactory
{
    IHandCardUI InstantiateCardView();
    void ReturnCard(IHandCardUI card_view);
    void ReturnCards();
}