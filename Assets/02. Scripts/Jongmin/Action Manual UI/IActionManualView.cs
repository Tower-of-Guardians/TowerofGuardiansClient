public interface IActionManualView
{
    void Inject(ActionManualPresenter presenter);
    void UpdateUI(ActionData action_data, bool can_action);
}