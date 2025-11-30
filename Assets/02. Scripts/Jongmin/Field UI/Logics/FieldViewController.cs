public class FieldViewController
{
    private readonly IFieldView m_view;
    private readonly INotice m_notice;

    public FieldViewController(IFieldView view,
                               INotice notice)
    {
        m_view = view;
        m_notice = notice;
    }

    public void ToggleManual(bool active)
        => m_view.ToggleManual(active);

    public void Notify(string notify_string)
        => m_notice.Notify(notify_string);
}
