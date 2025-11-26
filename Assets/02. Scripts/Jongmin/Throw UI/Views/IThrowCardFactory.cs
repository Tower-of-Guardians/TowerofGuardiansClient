public interface IThrowCardFactory
{
    IThrowCardView InstantiateCardView();
    void ReturnCard(IThrowCardView card_view, CardData card_data);
    void ReturnCards();
}