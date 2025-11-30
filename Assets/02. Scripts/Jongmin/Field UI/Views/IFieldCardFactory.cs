public interface IFieldCardFactory
{
    IFieldCardView InstantiateCardView();
    void ReturnCard(IFieldCardView card_view);
    void ReturnCards();
}