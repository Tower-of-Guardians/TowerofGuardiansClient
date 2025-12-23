public interface IBattleShopSlotView : ICardView
{
    void Inject(BattleShopSlotPresenter presenter);
    void InitUI(ShopCardData slot_data, bool can_purchase);
}