public interface ICardInventoryView
{
    void Inject(CardInventoryPresenter presenter);
    void OpenUI();
    void CloseUI();
}