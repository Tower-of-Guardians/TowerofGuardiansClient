public class HandCardViewController
{
    private readonly IHandView m_view;

    public HandCardViewController(IHandView view)
        => m_view = view;

    public void OpenUI()
        => m_view.OpenUI();

    public void CloseUI()
        => m_view.CloseUI();
}
