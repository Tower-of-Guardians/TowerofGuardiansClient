public class CardInventoryPresenter
{
    private readonly ICardInventoryView m_view;
    private readonly CardInventoryService m_service;

    public CardInventoryPresenter(ICardInventoryView view,
                                  CardInventoryFactory factory)
    {
        m_view = view;
        m_service = new CardInventoryService(new CardInventoryContainer(),
                                             factory);
    }

    public void OpenUI()
    {
        m_view.OpenUI();
        GetCards();
    }

    public void CloseUI()
    {
        m_view.CloseUI();
        ReturnCards();
    }

    private void GetCards()
    {
        DataCenter.Instance.SortUserCards(SortType.Grade);
        foreach(var card_data in DataCenter.Instance.userDeck)
            m_service.Add(card_data);
    }

    private void ReturnCards()
    {
        m_service.RemoveAll();
    }
}
