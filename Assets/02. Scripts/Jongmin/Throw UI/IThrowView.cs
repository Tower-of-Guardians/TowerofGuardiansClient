public interface IThrowView
{
    void Inject(ThrowPresenter presenter);

    void OpenUI();
    void UpdateUI(bool open_active, bool throw_active);
    void ThrowUI();
    void CloseUI();

    void ToggleManual(bool active);
}