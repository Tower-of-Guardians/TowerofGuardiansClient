public interface IMerchantShopView
{
    void Inject(MerchantShopPresenter presenter);
    void OpenUI();
    void CloseUI();
    void ToggleSaleButton(bool active);
}