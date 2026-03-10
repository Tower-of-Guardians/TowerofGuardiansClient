public interface IBattleShopSlotView : ICardUI
{
    void Inject(BattleShopSlotPresenter presenter);
    void InitUI(ShopCardData slot_data, bool can_purchase);
}