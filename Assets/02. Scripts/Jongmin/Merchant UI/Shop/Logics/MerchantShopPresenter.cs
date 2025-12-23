public class MerchantShopPresenter
{
    private readonly IMerchantShopView m_view;
    private readonly MerchantShopDispenser m_dispenser;

    public MerchantShopPresenter(IMerchantShopView view,
                                 MerchantShopDispenser dispenser)
    {
        m_view = view;
        m_dispenser = dispenser;
    }

    public void OpenUI()
    {
        m_view.OpenUI();
        m_dispenser.Initialize();
    }

    public void CloseUI()
        => m_view.CloseUI();
}
