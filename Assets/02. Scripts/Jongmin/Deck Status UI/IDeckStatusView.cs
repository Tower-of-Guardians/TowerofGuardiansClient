public interface IDeckStatusView
{
    void Inject(DeckStatusPresenter presenter);

    void OpenUI();
    void UpdateUI(string title_string);
    void UpdateThrowCardCount(int count);
    void UpdateDrawCardCount(int count);
    void CloseUI();

    IDeckStatusCardView InstantiateCardView();
    void ReturnCards();
}