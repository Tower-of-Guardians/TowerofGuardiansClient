using System;

public class StatusPresenter : IDisposable
{
    private readonly IStatusView m_view;

    public StatusPresenter(IStatusView view)
    {
        m_view = view;

        DataCenter.Instance.playerLevelEvent += UpdateLevel;
        DataCenter.Instance.playerMoneyEvent += UpdateGold;

        DataCenter.Instance.SetPlayerState();
        
        m_view.Inject(this);
    }

    public void UpdateLevel(int level, int exp)
    {
        float exp_ratio = (float)exp / DataCenter.Instance.playerstate.maxexperience;
        m_view.UpdateLevel(level, exp_ratio);
    }

    public void UpdateGold(int gold)
        => m_view.UpdateGold(gold);

    public void Dispose()
    {
        if(DataCenter.Instance != null)
        {
            DataCenter.Instance.playerLevelEvent -= UpdateLevel;
            DataCenter.Instance.playerMoneyEvent -= UpdateGold;
        }
    }
}
