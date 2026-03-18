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

    public void Inject(BattleCardData card_data)
    {
        BattleCardData = card_data;
        Purchased = false;

        m_dispenser.OnPurchasedAnyItem += UpdateUI;
        UpdateUI();
    }

    public void OnClickedPurchase()
    {
        var card_cost = BattleCardData.data.price;
        var can_purchase = m_player_state.money >= card_cost;

        if(can_purchase)
        {
            m_player_state.money -= (int)card_cost;
            DataCenter.Instance.userDeck.Add(BattleCardData.data);

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
        var card_cost = BattleCardData.data.price;
        
        var can_purchase = m_player_state.money >= card_cost;
        m_view.UpdateUI(BattleCardData, can_purchase);
    }
}
