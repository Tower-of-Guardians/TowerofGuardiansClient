using UnityEngine;

public class TurnRuleServiceInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("턴 규칙 디자이너")]
    [SerializeField] private TurnRuleDesigner m_turn_rule_designer;
    
    public void Inject()
    {
        InjectService();
    }

    private void InjectService()
    {
        DIContainer.Register<ITurnRuleService>(m_turn_rule_designer);
    }
}
