using UnityEngine;

[CreateAssetMenu(fileName = "Turn Rule Designer", menuName = "SO/Design/Turn Rule Designer")]
public class TurnRuleDesigner : ScriptableObject, ITurnRuleService
{
    [Header("턴 규칙 목록")]
    [SerializeField] private TurnRuleData[] m_turn_rule_data;

    public TurnRuleData GetRule(int card_count)
    {
        foreach(var rule in m_turn_rule_data)
            if(rule.Min <= card_count && card_count < rule.Max)
                return rule;

        return null;
    }
}
