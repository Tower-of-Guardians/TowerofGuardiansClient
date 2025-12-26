public interface IReinforcementCardView : ICardView
{
    void UpgradeATK(float atk);
    void UpgradeBoth(float atk, float def);
    void UpgradeDEF(float def);
}