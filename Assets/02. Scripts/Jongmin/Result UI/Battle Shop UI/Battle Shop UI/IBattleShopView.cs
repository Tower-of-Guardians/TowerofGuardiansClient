public interface IBattleShopView
{
    void Inject(BattleShopPresenter presenter);

    void OpenUI();
    void UpdateRate(string rate_string);
    void CloseUI();
}