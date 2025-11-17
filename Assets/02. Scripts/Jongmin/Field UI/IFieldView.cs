using UnityEngine.EventSystems;

public interface IFieldView : IDropHandler
{
    void Inject(FieldPresenter presenter);
    void ToggleManual(bool active);
    void PrintNotice(string notice_text);
    IFieldCardView InstantiateCardView();
    void ReturnCards();
}