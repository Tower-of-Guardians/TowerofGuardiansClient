public class SelectCardsBehavior : ICardBehavior
{
    public void OnClick(InventoryCardPresenter card_presenter)
        => card_presenter.ToggleSelectRequest();

    public void OnPointerEnter(InventoryCardPresenter card_presenter) {}

    public void OnPointerExit(InventoryCardPresenter card_presenter) {}
}
