using System;

public class ShopCardPresenter : CardPresenter, IDisposable
{
    private readonly IShopCardView m_view;
    // TODO: 플레이어 데이터
    private readonly MerchantShopDispenser m_dispenser;

    public bool Purchased { get; private set; }

    public ShopCardPresenter(IShopCardView view,
                             MerchantShopDispenser dispenser)
    {
        m_view = view;
        m_dispenser = dispenser;

        m_view.Inject(this);
    }

    public void Inject(ShopCardData card_data)
    {
        m_card_data = card_data.Card;
        Purchased = false;

        m_dispenser.OnPurchasedAnyItem += UpdateUI;
        UpdateUI();
    }

    public override void Return()
        => m_view.Return();

    public void OnClickedPurchase()
    {
        var shop_card_data = new ShopCardData(m_card_data);
        var card_cost = shop_card_data.Cost;
        // TODO: 플레이어의 골드와 카드의 비용을 대소 비교
        var can_purchase = true;

        if(can_purchase)
        {
            // TODO: 플레이어 데이터에서 골드 차감
            // TODO: 플레이어의 인벤토리에 카드 추가

            Purchased = true;
            m_dispenser.Alert();
        }
    }

    public void Dispose()
    {
        if(m_dispenser != null)
            m_dispenser.OnPurchasedAnyItem -= UpdateUI;
    }

    private void UpdateUI()
    {
        var shop_card_data = new ShopCardData(m_card_data);
        var card_cost = shop_card_data.Cost;
        
        // TODO: 플레이어의 골드와 카드의 비용을 대소 비교
        var can_purchase = true;
        m_view.UpdateUI(shop_card_data, can_purchase);
    }
}
