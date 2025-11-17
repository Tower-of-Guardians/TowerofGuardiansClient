public interface IRewardView
{
    void Inject(RewardPresenter presenter);

    void OpenUI(int gold, int exp, bool is_level_up);
    void CloseUI();
}