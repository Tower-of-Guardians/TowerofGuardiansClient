public interface IBattleShopSlotView
{
    void Inject(BattleShopSlotPresenter presenter);
    void InitUI(BattleShopSlotData slot_data, bool can_purchase);
}