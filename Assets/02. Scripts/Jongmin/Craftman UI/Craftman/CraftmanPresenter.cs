public class CraftmanPresenter
{
    private readonly ICraftmanView m_view;
    private readonly CraftmanInventoryPresenter m_inventory_presenter;

    public CraftmanPresenter(ICraftmanView view,
                             CraftmanInventoryPresenter inventory_presenter)
    {
        m_view = view;
        m_inventory_presenter = inventory_presenter;
    
        m_view.Inject(this);
    }

    public void OpenUI()
    {
        m_view.OpenUI();
        m_inventory_presenter.OpenUI();
    }

    public void CloseUI()
    {
        m_view.CloseUI();
        m_inventory_presenter.CloseUI();
    }
}
