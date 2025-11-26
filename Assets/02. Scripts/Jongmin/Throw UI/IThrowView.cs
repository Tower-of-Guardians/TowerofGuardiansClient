using UnityEngine.EventSystems;

public interface IThrowView : IDropHandler
{
    void Inject(ThrowPresenter presenter);

    void OpenUI();
    void UpdateUI(bool open_active, bool throw_active);
    void ThrowUI();
    void CloseUI();

    void ToggleManual(bool active);
    IThrowCardView InstantiateCardView();
    void ReturnCard(IThrowCardView card_view, CardData card_data);
    void ReturnCards();
    void PrintNotice(string notice_text);
}