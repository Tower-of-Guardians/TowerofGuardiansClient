public class ReinforcementPresenter
{
    private readonly IReinforcementView m_view;
    private readonly ReinforcementCardPresenter m_card_presenter;
    private readonly IReinforcementDataBase m_reinforcement_db;
    private CraftmanPresenter m_craftman_presenter;
    private CraftmanInventoryPresenter m_craftman_inventory_presenter;

    public ReinforcementPresenter(IReinforcementView view,
                                  ReinforcementCardPresenter card_presenter,
                                  IReinforcementDataBase reinforcement_db)
    {
        m_view = view;
        m_card_presenter = card_presenter;
        m_reinforcement_db = reinforcement_db;

        m_view.Inject(this);
    }

    public void Inject(CraftmanPresenter craftman_presenter,
                       CraftmanInventoryPresenter craftman_inventory_presenter)
    {
        m_craftman_presenter = craftman_presenter;
        m_craftman_inventory_presenter = craftman_inventory_presenter;
    }

    public void OpenUI(CardData card_data)
    {
        m_card_presenter.Inject(card_data);
        m_view.OpenUI();
        m_view.ToggleButtonGroup(true);
    }

    public void CloseUI()
        => m_view.CloseUI();

    public void OnClickedAtkUpgrade()
    {
        // TODO: 현재 스테이지만큼으로 조정
        var reinforcement_data = m_reinforcement_db.GetReinforcementData(1);
        m_card_presenter.ATKUpgrade(reinforcement_data.ATK);

        m_craftman_inventory_presenter.UpdateEnforcedBubble();
        m_view.ToggleCloseButton(true);
        m_view.ToggleButtonGroup(false);
    }

    public void OnClickedBothUpgrade()
    {
        // TODO: 현재 스테이지만큼으로 조정
        var reinforcement_data = m_reinforcement_db.GetReinforcementData(1);
        m_card_presenter.BothUpgrade(reinforcement_data.ATK * 0.5f, reinforcement_data.DEF * 0.5f);

        m_craftman_inventory_presenter.UpdateEnforcedBubble();
        m_view.ToggleCloseButton(true);
        m_view.ToggleButtonGroup(false);
    }

    public void OnClickedDefUpgrade()
    {
        // TODO: 현재 스테이지만큼으로 조정
        var reinforcement_data = m_reinforcement_db.GetReinforcementData(1);
        m_card_presenter.DEFUpgrade(reinforcement_data.DEF);

        m_craftman_inventory_presenter.UpdateEnforcedBubble();
        m_view.ToggleCloseButton(true);
        m_view.ToggleButtonGroup(false);
    }

    public void OnClickedCancel()
    {
        m_craftman_inventory_presenter.OpenUI();
        m_view.CloseUI();
    }

    public void OnClickedClose()
    {
        m_craftman_inventory_presenter.CloseUI();
        m_view.CloseUI();

        m_craftman_presenter.CloseUI();
    }
}
