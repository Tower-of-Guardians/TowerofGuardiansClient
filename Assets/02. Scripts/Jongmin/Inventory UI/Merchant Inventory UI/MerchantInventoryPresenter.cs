public class MerchantInventoryPresenter : CardInventoryPresenter
{
    private MerchantShopPresenter m_shop_presenter;
    private MerchantDialogueBubblePresenter m_dialogue_bubble_presenter;

    public MerchantInventoryPresenter(ICardInventoryView view, 
                                      CardInventoryFactory factory,
                                      ICardBehavior behavior,
                                      INotice notice,
                                      MerchantDialogueBubblePresenter dialogue_bubble_presenter) 
        : base(view, factory, behavior, notice)
    {
        m_dialogue_bubble_presenter = dialogue_bubble_presenter;
        m_view.Inject(this);
    }

    public void Inject(MerchantShopPresenter shop_presenter)
        => m_shop_presenter = shop_presenter;

    public override void OpenUI()
    {
        m_dialogue_bubble_presenter.OpenUI(this);
        AlertUpdateSelectedCards();
        base.OpenUI();
    }

    public override void CloseUI()
    {
        m_dialogue_bubble_presenter.CloseUI(this);
        base.CloseUI();
    }

    public void OnClickedSale()
    {
        foreach(var card in m_selected_cards)
            DataCenter.Instance.userDeck.Remove(card.CardData);
        
        var total_money = GetSalePrice();
        DataCenter.Instance.playerstate.money -= total_money;

        m_shop_presenter.ToggleSaleButton(false);

        ReturnCards();
        GetCards();

        OnClickedBack();
    }

    public void OnClickedBack()
    {
        m_shop_presenter.FadeUpUI();
        CloseUI();
    }

    protected override ICardSelectionRequester CreateSelectionRequester()
        => this;
}
