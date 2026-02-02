public class ThrowViewController
{
    private readonly IThrowView m_view;
    private readonly INotice m_notice;

    public bool Active { get; private set; }
    public ThrowViewController(IThrowView view,
                               INotice notice)
    {
        m_view = view;
        m_notice = notice;
    }
    
    public void OpenUI()
    {
        Active = true;
        m_view.OpenUI();
    }

    public void CloseUI()
    {
        Active = false;
        m_view.CloseUI();
    }

    public void ThrowUI()
    {
        Active = false;
        m_view.ThrowUI();
    }

    public void ToggleManual(bool active)
        => m_view.ToggleManual(active);

    public void UpdateThrowState(bool active)
        => m_view.UpdateOpenButton(active);
    
    public void UpdateThrowCount(ActionData data)
        => m_view.UpdateThrowButton(data.Current > 0);
    
    public void Notify(string notify_message)
        => m_notice.Notify(notify_message);
}
