public interface IHandView
{
    void Inject(HandPresenter presenter);

    void OpenUI();
    void CloseUI();
}