public class MerchantShopPresenter
{
    private readonly IMerchantShopView m_view;
    private readonly MerchantShopDispenser m_dispenser;
    private readonly MerchantDeckInvenPresenter _mDeckInvenPresenter;

    public MerchantShopPresenter(IMerchantShopView view,
                                 MerchantShopDispenser dispenser,
                                 MerchantDeckInvenPresenter deckInvenPresenter)
    {
        m_view = view;
        m_dispenser = dispenser;
        _mDeckInvenPresenter = deckInvenPresenter;

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
        _mDeckInvenPresenter.OpenUI();
    }
}
