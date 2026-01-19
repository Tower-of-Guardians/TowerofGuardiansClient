using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    private ITurnRuleService m_turn_rule_service;
    private int m_current_action_count;
    private int m_current_throw_count;
    private bool m_is_can_throw;

    private HandPresenter m_hand_prsenter;

    [Header("에디터 테스트 옵션")]
    [Header("현재 보유 중인 카드의 수")]
    [SerializeField] private int m_card_count;

    public int CurrentActionCount
    {
        get => m_current_action_count;
        private set => m_current_action_count = value;
    }

    public int CurrentThrowCount
    {
        get => m_current_throw_count;
        private set => m_current_throw_count = value;
    }

    public int MaxThrowCount => MaxActionCount;

    public int MaxActionCount => m_turn_rule_service.GetRule(m_card_count).MaxUseCount;
    public int MaxHandCount => m_turn_rule_service.GetRule(m_card_count).MaxHandCount;

    public event Action<ActionData> OnUpdatedActionCount;
    public event Action<ActionData> OnUpdatedThrowCount;
    public event Action<bool> OnUpdatedThrowActionState;
    public event Action StartNewTurn;
    public event Action EndCurrentTurn;


    public void Inject(ITurnRuleService turn_rule_service)
    {
        m_turn_rule_service = turn_rule_service;

        Initialize();
    }

    public void Inject(HandPresenter hand_presenter)
        => m_hand_prsenter = hand_presenter;

    public void AlertToUpdateActionCount()
        => OnUpdatedActionCount?.Invoke(new ActionData(CurrentActionCount, MaxActionCount));

    public void AlertToUpdateThrowCount()
        => OnUpdatedThrowCount?.Invoke(new ActionData(CurrentThrowCount, MaxThrowCount));

    public void Initialize()
    {
        CurrentActionCount = 0;
        CurrentThrowCount = 0;
        UpdateThrowAction(true);

        AlertToUpdateActionCount();
        AlertToUpdateThrowCount();
    }

    public void UpdateActionCount(int count)
    {
        CurrentActionCount += count;
        CurrentActionCount = Mathf.Clamp(CurrentActionCount, 0, MaxActionCount);

        AlertToUpdateActionCount();
    }

    public void UpdateThrowCount(int count)
    {
        CurrentThrowCount += count;
        CurrentThrowCount = Mathf.Clamp(CurrentThrowCount, 0, MaxThrowCount);

        AlertToUpdateThrowCount();
    }

    public void UpdateThrowAction(bool active)
    {
        m_is_can_throw = active;
        OnUpdatedThrowActionState?.Invoke(m_is_can_throw);
    }

    public void StartTurn()
        => StartNewTurn?.Invoke();

    public void EndTurn()
        => EndCurrentTurn?.Invoke();

    public bool CanAction() 
        => CurrentActionCount < MaxActionCount;

    public bool CanThrow()
        => m_is_can_throw && CurrentThrowCount < MaxThrowCount;

#region Test
    public void BTN_Instantiate()
    {
        for(int i = 0; i < MaxHandCount; i++)
        {
            var card_data = GameData.Instance.NextDeckSet(1);

            m_hand_prsenter.InstantiateCard(card_data);
        }
    }
#endregion Test
}