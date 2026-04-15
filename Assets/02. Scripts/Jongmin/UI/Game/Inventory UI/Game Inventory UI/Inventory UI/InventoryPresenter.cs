public class InventoryPresenter
{
    private readonly IInventoryUI _inventoryUI;
    private readonly InventoryTabPresenter _tabPresenter;

    private bool _isActive;

    public InventoryPresenter(IInventoryUI inventoryUI,
                              InventoryTabPresenter tabPresenter)
    {
        _inventoryUI = inventoryUI;
        _tabPresenter = tabPresenter;

        tabPresenter.OnActivateTab += _inventoryUI.UpdateTitle;
        
        _inventoryUI.Construct(this);
    }

    public void ToggleUI()
    {
        _isActive = !_isActive;
        
        if (_isActive)
        {
            _inventoryUI.OpenUI();
            _tabPresenter.Initialize();
        }
        else
        {
            _inventoryUI.CloseUI();
        }
    }
}
