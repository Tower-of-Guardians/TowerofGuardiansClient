using System;

public class ActionManualPresenter : IDisposable
{
    private readonly IActionManualView m_view;
    private readonly TurnManager m_turn_manager;

    public ActionManualPresenter(IActionManualView view,
                                 TurnManager turn_manager)
    {
        m_view = view;
        m_turn_manager = turn_manager;

        m_turn_manager.OnUpdatedActionCount += UpdateUI;
        m_turn_manager.Initialize();
    }

    public void Dispose()
        => m_turn_manager.OnUpdatedActionCount -= UpdateUI;

    public void UpdateUI(ActionData action_data)
        => m_view.UpdateUI(action_data, m_turn_manager.CanAction());
}