using Unity.VisualScripting;

public class ResultPresenter
{
    private readonly IResultView m_view;
    private readonly RewardPresenter m_reward_presenter;
    private readonly BattleShopPresenter m_shop_presenter;
    private readonly CardInventoryPresenter m_inventory_presenter;

    public ResultPresenter(IResultView view,
                           RewardPresenter reward_presenter,
                           BattleShopPresenter shop_presenter,
                           CardInventoryPresenter inventory_presenter)
    {
        m_view = view;
        m_reward_presenter = reward_presenter;
        m_shop_presenter = shop_presenter;
        m_inventory_presenter = inventory_presenter;

        m_view.Inject(this);
    }

    public void OpenUI(ResultData result_data)
    {
        m_view.OpenUI(result_data.Type == BattleResultType.Victory);

        if(result_data.Type == BattleResultType.Victory)
        {
            m_reward_presenter.OpenUI(result_data.Gold, 
                                      result_data.EXP);

            m_shop_presenter.OpenUI();
        }
    }

    public void CloseUI()
    {
        m_view.CloseUI();
        m_reward_presenter.CloseUI();
        m_shop_presenter.CloseUI();
        m_inventory_presenter.CloseUI();
    }

    public void OpenInventory()
        => m_inventory_presenter.OpenUI();
}
