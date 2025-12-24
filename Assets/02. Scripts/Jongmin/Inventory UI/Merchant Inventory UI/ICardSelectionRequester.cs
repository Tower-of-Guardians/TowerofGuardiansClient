public interface ICardSelectionRequester
{
    bool RequestSelect(InventoryCardPresenter card_presenter);
    void RequestDeselect(InventoryCardPresenter card_presenter);
}