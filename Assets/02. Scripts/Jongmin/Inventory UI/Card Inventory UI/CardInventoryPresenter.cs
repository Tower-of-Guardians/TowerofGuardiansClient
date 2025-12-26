using System;
using System.Collections.Generic;

public class CardInventoryPresenter : ICardSelectionRequester
{
    protected readonly ICardInventoryView m_view;
    protected readonly CardInventoryService m_service;
    protected readonly INotice m_notice;

    protected readonly HashSet<InventoryCardPresenter> m_selected_cards = new();
    protected readonly int MAX_SELECT = 3;

    public event Action<int, int, int> OnSelectedCardsChanged;

    public CardInventoryPresenter(ICardInventoryView view,
                                  CardInventoryFactory factory,
                                  ICardBehavior behavior,
                                  INotice notice)
    {
        m_view = view;
        m_notice = notice;
        m_service = new CardInventoryService(new CardInventoryContainer(),
                                             factory,
                                             behavior);

        m_service.SetSelectionRequester(CreateSelectionRequester());
        m_service.SetSelectionController(CreateSelectionController());
    }

    public virtual void OpenUI()
    {
        m_view.OpenUI();
        GetCards();
    }

    public virtual void CloseUI()
    {
        m_view.CloseUI();
        ReturnCards();
    }

    public bool RequestSelect(InventoryCardPresenter card_presenter)
    {
        if(m_selected_cards.Count >= MAX_SELECT)
        {
            m_notice.Notify("<color=red>카드는 최대 3장까지 판매할 수 있습니다.</color>");
            return false;
        }

        m_selected_cards.Add(card_presenter);
        card_presenter.SetSelected(true);

        AlertUpdateSelectedCards();        

        return true;
    }

    public void RequestDeselect(InventoryCardPresenter card_presenter)
    {
        if(m_selected_cards.Remove(card_presenter))
        {
            card_presenter.SetSelected(false);
            AlertUpdateSelectedCards();
        }
    }

    protected void GetCards()
    {
        DataCenter.Instance.SortUserCards(SortType.Grade);
        foreach(var card_data in DataCenter.Instance.userDeck)
            m_service.Add(card_data);
    }

    protected void ReturnCards()
        => m_service.RemoveAll();

    protected virtual ICardSelectionRequester CreateSelectionRequester()
        => null;

    protected virtual ICardSelectionController CreateSelectionController()
        => null;

    protected void AlertUpdateSelectedCards()
        => OnSelectedCardsChanged?.Invoke(m_selected_cards.Count, MAX_SELECT, GetSalePrice());

    protected int GetSalePrice()
    {
        var total_money = 0;
        foreach(var selected_card in m_selected_cards)
            total_money += (int)selected_card.CardData.price;

        total_money = (int)(total_money * 0.8f);

        return total_money;
    }
}
