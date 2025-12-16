public class InventoryPresenter
{
    private readonly IInventoryView m_view;
    private readonly InventoryTabPresenter m_tab_presenter;

    public InventoryPresenter(IInventoryView view,
                              InventoryTabPresenter tab_presenter)
    {
        m_view = view;
        m_tab_presenter = tab_presenter;
        m_view.Inject(this);
    }

    public void OpenUI()
    {
        m_view.OpenUI();
        m_tab_presenter.Initialize();
    }

    public void CloseUI()
        => m_view.CloseUI();
}
