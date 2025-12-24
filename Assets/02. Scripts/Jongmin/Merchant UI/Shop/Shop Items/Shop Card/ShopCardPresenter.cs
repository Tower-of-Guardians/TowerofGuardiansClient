using System;

public class ShopCardPresenter : CardPresenter, IDisposable
{
    private readonly IShopCardView m_view;
    private readonly PlayerState m_player_state;
    private readonly MerchantShopDispenser m_dispenser;

    public bool Purchased { get; private set; }

    public ShopCardPresenter(IShopCardView view,
                             MerchantShopDispenser dispenser)
    {
        m_view = view;
        m_player_state = DataCenter.Instance.playerstate;
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
        var can_purchase = m_player_state.money >= card_cost;

        if(can_purchase)
        {
            m_player_state.money -= (int)card_cost;
            DataCenter.Instance.userDeck.Add(m_card_data.data);

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
        
        var can_purchase = m_player_state.money >= card_cost;
        m_view.UpdateUI(shop_card_data, can_purchase);
    }
}
