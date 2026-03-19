using System;
using System.Collections.Generic;

public sealed class MerchantDeckInvenPresenter : DeckInvenPresenter, ICardSelectionRequester
{
    private MerchantShopPresenter m_shop_presenter;
    private MerchantDialogueBubblePresenter m_dialogue_bubble_presenter;

    private readonly HashSet<DeckInvenCardPresenter> selectedCardSet = new();
    private const int MaxSelectCount = 3;

    public event Action<int, int, int> OnSelectedCardsChanged;

    public MerchantDeckInvenPresenter(IDeckInvenUI view, 
                                      ICardFactory<IDeckInvenCardUI> factory,
                                      CardContainer<IDeckInvenCardUI, DeckInvenCardPresenter> cardContainer,
                                      ICardBehavior behavior,
                                      MerchantDialogueBubblePresenter dialogue_bubble_presenter) 
        : base(view, factory, cardContainer, behavior)
    {
        m_dialogue_bubble_presenter = dialogue_bubble_presenter;
        _deckInvenUI.Construct(this);
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
        foreach (var cardPresenter in selectedCardSet)
        {
            DataCenter.Instance.userDeck.Remove(cardPresenter.CardData);
        }

        int totalMoney = GetTotalSalePrice();
        DataCenter.Instance.playerstate.money += totalMoney;

        m_shop_presenter.ToggleSaleButton(false);

        RemoveAllCards();
        CreateCardsFromDataCenter();
        OnClickedBack();
    }

    public void OnClickedBack()
    {
        m_shop_presenter.FadeUpUI();
        CloseUI();
    }

    protected override ICardSelectionRequester CreateSelectionRequester()
        => this;
    
    private void AlertUpdateSelectedCards()
        => OnSelectedCardsChanged?.Invoke(selectedCardSet.Count, MaxSelectCount, GetTotalSalePrice());

    private int GetTotalSalePrice()
    {
        int totalMoney = 0;
        
        foreach (DeckInvenCardPresenter cardPresenter in selectedCardSet)
        {
            totalMoney += cardPresenter.CardData.price;
        }

        totalMoney = (int)(totalMoney * 0.8f);
        return totalMoney;
    }

    public bool RequestSelect(DeckInvenCardPresenter deckInvenCardPresenter)
    {
        if (selectedCardSet.Count >= MaxSelectCount)
        {
            return false;
        }
        
        selectedCardSet.Add(deckInvenCardPresenter);
        deckInvenCardPresenter.SetSelected(true);
        AlertUpdateSelectedCards();

        return true;
    }

    public void RequestDeselect(DeckInvenCardPresenter deckInvenCardPresenter)
    {
        if (!selectedCardSet.Remove(deckInvenCardPresenter))
        {
            return;
        }
        
        deckInvenCardPresenter.SetSelected(false);
        AlertUpdateSelectedCards();
    }
}
