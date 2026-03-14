using System;
using VContainer.Unity;

public class StatusPresenter : IDisposable, IInitializable
{
    private readonly IStatusUI _statusUI;

    public StatusPresenter(IStatusUI statusUI)
        => _statusUI = statusUI;
    
    public void Initialize()
    {
        DataCenter.Instance.playerLevelEvent += UpdateLevel;
        DataCenter.Instance.playerMoneyEvent += UpdateGold;
        DataCenter.Instance.SetPlayerState();
    }

    public void UpdateLevel(int level, int exp)
    {
        float expRatio = (float)exp / DataCenter.Instance.playerstate.maxexperience;
        _statusUI.UpdateLevel(level, expRatio);
    }

    public void UpdateGold(int gold)
        => _statusUI.UpdateGold(gold);

    public void Dispose()
    {
        if (DataCenter.Instance == null)
        {
            return;
        }
        
        DataCenter.Instance.playerLevelEvent -= UpdateLevel;
        DataCenter.Instance.playerMoneyEvent -= UpdateGold;
    }
}
