public interface IResultView
{
    void Inject(ResultPresenter presenter);
    
    void OpenUI(bool is_victory);
    void CloseUI();
}