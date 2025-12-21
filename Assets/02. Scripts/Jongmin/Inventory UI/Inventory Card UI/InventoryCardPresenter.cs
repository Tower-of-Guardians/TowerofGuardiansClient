public class InventoryCardPresenter : CardPresenter
{    
    private readonly IInventoryCardView m_view;
    private readonly new CardData m_card_data;

    public InventoryCardPresenter(IInventoryCardView view,
                                  CardData card_data)
    {
        m_view = view;
        m_card_data = card_data;

        m_view.InitUI(card_data);
    }

    public override void Return()
        => m_view.Return();
}
