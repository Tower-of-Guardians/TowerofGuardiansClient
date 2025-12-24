using System;

public class ShopPotionPresenter : IDisposable
{
    private readonly IShopPotionView m_view;
    private readonly PlayerState m_player_state;
    private readonly MerchantShopDispenser m_dispenser;

    private readonly int COST = 40;
    private readonly float RECOVERY_RATE = 0.2f;

    public bool Purchased { get; private set; }

    public ShopPotionPresenter(IShopPotionView view,
                               MerchantShopDispenser dispenser)
    {
        m_view = view;
        m_player_state = DataCenter.Instance.playerstate;
        m_dispenser = dispenser;

        m_view.Inject(this);
    }

    public void Initialize()
    {
        Purchased = false;

        m_dispenser.OnPurchasedAnyItem += UpdateUI;
        UpdateUI();
    }

    public void OnClickedPurchase()
    {
        if(m_player_state.money < COST)
            return;

        Purchased = true;

        m_player_state.money -= COST;
        m_player_state.hp = (int)(m_player_state.hp * RECOVERY_RATE);

        m_dispenser.Alert();
    }

    public void Dispose()
    {
        if(m_dispenser != null)
            m_dispenser.OnPurchasedAnyItem -= UpdateUI;
    }

    private void UpdateUI()
    {
        var can_purchase = m_player_state.money >= COST;
        m_view.UpdateUI(COST, can_purchase);
    }
}
