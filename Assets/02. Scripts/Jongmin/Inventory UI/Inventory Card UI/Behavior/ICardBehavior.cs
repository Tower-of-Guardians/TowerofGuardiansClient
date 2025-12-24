public interface ICardBehavior
{
    void OnClick(InventoryCardPresenter card_presenter);
    void OnPointerEnter(InventoryCardPresenter card_presenter);
    void OnPointerExit(InventoryCardPresenter card_presenter);
}