public interface IInventorySortView
{
    void Inject(InventorySortPresenter presenter);
    void UpdateSortLabel(string sort_text);
    void UpdateSortButton(bool is_ascending);
}