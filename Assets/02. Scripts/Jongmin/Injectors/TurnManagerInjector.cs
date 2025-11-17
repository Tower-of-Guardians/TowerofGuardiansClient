using UnityEngine;

public class TurnManagerInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("턴 관리자")]
    [SerializeField] private TurnManager m_turn_manager;

    public void Inject()
    {
        InjectManager();
    }

    private void InjectManager()
    {
        DIContainer.Register<TurnManager>(m_turn_manager);
        m_turn_manager.Inject(DIContainer.Resolve<ITurnRuleService>());
    }
}
