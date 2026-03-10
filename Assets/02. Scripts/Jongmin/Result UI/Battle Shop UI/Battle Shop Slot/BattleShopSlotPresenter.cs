public class BattleShopSlotPresenter : CardPresenter
{
    private readonly IBattleShopSlotView m_view;

    public BattleShopSlotPresenter(IBattleShopSlotView view, BattleCardData card_data)
    {
        m_view = view;
        BattleCardData = card_data;

        m_view.UpdateUI(card_data.data);
    }

    public BattleShopSlotPresenter(IBattleShopSlotView view, ShopCardData slot_data)
    {
        m_view = view;
        BattleCardData = slot_data.Card;

        m_view.InitUI(slot_data, true);
    }
}
