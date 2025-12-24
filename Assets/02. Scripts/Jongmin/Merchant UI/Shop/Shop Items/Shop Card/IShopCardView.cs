public interface IShopCardView : ICardView
{
    void Inject(ShopCardPresenter presenter);
    void InitUI(ShopCardData card_data, bool can_purchase);
    void UpdateUI(ShopCardData card_data, bool can_purchase);
}