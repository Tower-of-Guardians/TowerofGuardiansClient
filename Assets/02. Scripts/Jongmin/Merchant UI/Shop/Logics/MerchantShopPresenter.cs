public class MerchantShopPresenter
{
    private readonly IMerchantShopView m_view;
    private readonly MerchantShopDispenser m_dispenser;
    private readonly MerchantInventoryPresenter m_inventory_presenter;

    public MerchantShopPresenter(IMerchantShopView view,
                                 MerchantShopDispenser dispenser,
                                 MerchantInventoryPresenter inventory_presenter)
    {
        m_view = view;
        m_dispenser = dispenser;
        m_inventory_presenter = inventory_presenter;

        m_view.Inject(this);
    }

    public void OpenUI()
    {
        m_view.ToggleSaleButton(true);
        m_view.OpenUI();
        m_dispenser.Initialize();
    }

    public void CloseUI()
        => m_view.CloseUI();

    public void FadeUpUI()
        => m_view.OpenUI();

    public void FadeDownUI()
        => m_view.CloseUI();

    public void ToggleSaleButton(bool active)
        => m_view.ToggleSaleButton(active);

    public void OnClickedSale()
    {
        FadeDownUI();
        m_inventory_presenter.OpenUI();
    }
}
