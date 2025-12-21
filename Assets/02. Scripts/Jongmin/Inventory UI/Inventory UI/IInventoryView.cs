public interface IInventoryView
{
    void Inject(InventoryPresenter presenter);
    void OpenUI();
    void CloseUI();
}