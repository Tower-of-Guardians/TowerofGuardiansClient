public interface IHandCardFactory
{
    IHandCardView InstantiateCardView();
    void ReturnCard(IHandCardView card_view);
    void ReturnCards();
}