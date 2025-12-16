public interface IBattleShopView
{
    void Inject(BattleShopPresenter presenter);

    void OpenUI();
    void UpdateRate(string rate_string);
    void UpdateRefresh(string refresh_string, bool can_refresh);
    void CloseUI();
}