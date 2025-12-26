public class ReinforcementCardPresenter : CardPresenter
{
    private IReinforcementCardView m_view;
    private new CardData m_card_data;

    public ReinforcementCardPresenter(IReinforcementCardView view)
        => m_view = view;

    public void Inject(CardData card_data)
    {
        m_card_data = card_data;
        m_view.InitUI(m_card_data);
    }

    public void ATKUpgrade(float atk)
    {
        m_card_data.ATK += atk;
        m_view.UpgradeATK(m_card_data.ATK);
    }

    public void BothUpgrade(float atk, float def)
    {
        m_card_data.ATK += atk;
        m_card_data.DEF += def;
        m_view.UpgradeBoth(m_card_data.ATK, m_card_data.DEF);
    }

    public void DEFUpgrade(float def)
    {
        m_card_data.DEF += def;
        m_view.UpgradeDEF(m_card_data.DEF);
    }

    public override void Return()
        => m_view.Return();
}
