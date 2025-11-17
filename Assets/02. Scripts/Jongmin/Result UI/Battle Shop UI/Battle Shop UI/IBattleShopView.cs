public interface IBattleShopView
{
    void Inject(BattleShopPresenter presenter);

    IBattleShopSlotView InstantiateSlotView();

    void OpenUI();
    void CloseUI();
}