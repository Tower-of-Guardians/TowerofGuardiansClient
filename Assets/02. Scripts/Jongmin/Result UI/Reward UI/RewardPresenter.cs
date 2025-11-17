public class RewardPresenter
{
    private readonly IRewardView m_view;
    // TODO: 플레이어 데이터 추가

    public bool LevelUp { get; private set; }

    public RewardPresenter(IRewardView view)
    {
        m_view = view;
        m_view.Inject(this);
    }

    public void OpenUI(int gold, int exp)
    {
        // TODO: 플레이어 데이터 조작(골드 및 경험치)
        m_view.OpenUI(gold, exp, false);
    }

    public void CloseUI()
    {
        m_view.CloseUI();
    }
}