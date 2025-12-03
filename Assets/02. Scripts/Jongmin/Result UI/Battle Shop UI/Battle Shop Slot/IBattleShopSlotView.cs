public interface IBattleShopSlotView : ICardView
{
    void Inject(BattleShopSlotPresenter presenter);
    void InitUI(BattleShopSlotData slot_data, bool can_purchase);
}