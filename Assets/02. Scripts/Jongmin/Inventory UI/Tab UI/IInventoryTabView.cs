public interface IInventoryTabView
{
    void Inject(InventoryTabPresenter presenter);
    void Initialize();
    void UpdateCardToggle(bool isOn);
    void UpdateMagicToggle(bool isOn);
}