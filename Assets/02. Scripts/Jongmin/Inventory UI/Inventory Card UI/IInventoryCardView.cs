public interface IInventoryCardView : ICardView
{
    void Inject(InventoryCardPresenter presenter);
    void ShowHighlight(bool active);
}