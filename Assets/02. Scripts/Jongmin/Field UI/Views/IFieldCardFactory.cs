public interface IFieldCardFactory
{
    IFieldCardUI InstantiateCardView();
    void ReturnCard(IFieldCardUI card_view);
    void ReturnCards();
}