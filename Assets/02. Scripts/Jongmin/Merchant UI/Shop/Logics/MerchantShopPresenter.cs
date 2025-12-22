using UnityEngine;

public class MerchantShopPresenter
{
    private readonly IMerchantShopView m_view;

    public MerchantShopPresenter(IMerchantShopView view)
        => m_view = view;

    public void OpenUI()
        => m_view.OpenUI();

    public void CloseUI()
        => m_view.CloseUI();
}
