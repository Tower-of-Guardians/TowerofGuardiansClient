public interface IShopCardView : ICardUI
{
    void Inject(ShopCardPresenter presenter);
    void InitUI(BattleCardData card_data, bool can_purchase);
    void UpdateUI(BattleCardData card_data, bool can_purchase);
}