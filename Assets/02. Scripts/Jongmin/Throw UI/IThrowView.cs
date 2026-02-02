public interface IThrowView
{
    void Inject(ThrowPresenter presenter);

    void OpenUI();
    void UpdateOpenButton(bool open_active);
    void UpdateThrowButton(bool throw_active);
    void ThrowUI();
    void CloseUI();

    void ToggleManual(bool active);
}