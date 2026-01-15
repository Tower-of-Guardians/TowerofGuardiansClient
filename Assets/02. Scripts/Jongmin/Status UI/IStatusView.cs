public interface IStatusView
{
    void Inject(StatusPresenter presenter);
    
    void UpdateLevel(int level, float exp);
    void UpdateGold(int gold);
}