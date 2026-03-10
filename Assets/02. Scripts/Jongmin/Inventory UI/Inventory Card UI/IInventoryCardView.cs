public interface IInventoryCardView : ICardUI
{
    void Inject(InventoryCardPresenter presenter);
    void ShowHighlight(bool active);
}