using UnityEngine;

public class ResultUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("보상 뷰")]
    [SerializeField] private RewardView m_reward_view;

    [Header("상점 뷰")]
    [SerializeField] private BattleShopView m_shop_view;

    [Header("결과 뷰")]
    [SerializeField] private ResultView m_result_view; 

    public void Inject()
    {
        InjectReward();
        InjectShop();
        InjectResult();
    }

    private void InjectReward()
    {
        DIContainer.Register<RewardView>(m_reward_view);

        var reward_presenter = new RewardPresenter(m_reward_view);
        DIContainer.Register<RewardPresenter>(reward_presenter);
    }

    private void InjectShop()
    {
        DIContainer.Register<IBattleShopView>(m_shop_view);

        var shop_presenter = new BattleShopPresenter(m_shop_view);
        DIContainer.Register<BattleShopPresenter>(shop_presenter);
    }

    private void InjectResult()
    {
        DIContainer.Register<ResultView>(m_result_view);

        var result_presenter = new ResultPresenter(m_result_view,
                                                   DIContainer.Resolve<RewardPresenter>(),
                                                   DIContainer.Resolve<BattleShopPresenter>());
        DIContainer.Register<ResultPresenter>(result_presenter);
    }
}