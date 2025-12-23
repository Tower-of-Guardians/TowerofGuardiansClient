public interface IShopPotionView
{
    void Inject(ShopPotionPresenter presenter);
    void InitUI(int cost, bool can_purchase);
    void UpdateUI(int cost, bool can_purchase);
}