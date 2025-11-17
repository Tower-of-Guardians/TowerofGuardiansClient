using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    private ITurnRuleService m_turn_rule_service;
    private int m_current_action_count;

    [Header("에디터 테스트 옵션")]
    [Header("현재 보유 중인 카드의 수")]
    [SerializeField] private int m_card_count;

    public int CurrentActionCount
    {
        get => m_current_action_count;
        private set => m_current_action_count = value;
    }

    public int MaxActionCount => m_turn_rule_service.GetRule(m_card_count).MaxUseCount;
    public int MaxHandCount => m_turn_rule_service.GetRule(m_card_count).MaxHandCount;

    public event Action<ActionData> OnUpdatedActionCount;

    public void Inject(ITurnRuleService turn_rule_service)
    {
        m_turn_rule_service = turn_rule_service;

        Initialize();
    }

    public void Alert()
        => OnUpdatedActionCount?.Invoke(new ActionData(CurrentActionCount, MaxActionCount));

    public void Initialize()
    {
        CurrentActionCount = 0;

        Alert();
    }

    public void UpdateActionCount(int count)
    {
        CurrentActionCount += count;
        CurrentActionCount = Mathf.Clamp(CurrentActionCount, 0, MaxActionCount);

        Alert();
    }

    public bool CanAction() 
        => CurrentActionCount < MaxActionCount;
}