public class BattleShopSlotPresenter : CardPresenter
{
    private readonly IBattleShopSlotView m_view;

    public BattleShopSlotPresenter(IBattleShopSlotView view, BattleCardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(m_card_data.data);
    }

    public BattleShopSlotPresenter(IBattleShopSlotView view, ShopCardData slot_data)
    {
        m_view = view;
        m_card_data = slot_data.Card;

        m_view.InitUI(slot_data, true);
    }

    public override void Return()
        => m_view.Return();
}
