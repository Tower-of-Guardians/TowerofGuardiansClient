public interface IHandView
{
    void Inject(HandPresenter presenter);

    IHandCardView InstantiateCardView();

    void OpenUI();
    void UpdateUI();
    void CloseUI();
}