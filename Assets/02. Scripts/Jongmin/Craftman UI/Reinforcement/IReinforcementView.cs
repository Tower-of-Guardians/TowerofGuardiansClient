public interface IReinforcementView
{
    void Inject(ReinforcementPresenter presenter);
    void OpenUI();
    void CloseUI();
    void ToggleCloseButton(bool active);
    void ToggleButtonGroup(bool active);
}