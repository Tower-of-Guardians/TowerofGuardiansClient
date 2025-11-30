public interface IFieldView
{
    void Inject(FieldPresenter presenter);
    void ToggleManual(bool active);
}