public class DialogueBubblePresenter
{
    private readonly IDialogueBubbleView m_view;

    public DialogueBubblePresenter(IDialogueBubbleView view)
        => m_view = view;

    public void OpenUI()
        => m_view.OpenUI();

    public void CloseUI()
        => m_view.CloseUI();

    public void SetBubble(string dialogue_string)
        => m_view.SetBubble(dialogue_string);
}
