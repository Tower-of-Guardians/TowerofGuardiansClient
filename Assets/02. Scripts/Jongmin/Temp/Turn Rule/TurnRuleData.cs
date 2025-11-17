using UnityEngine;

[System.Serializable]
public class TurnRuleData
{
    [Header("조건")]
    [Header("조건의 최소 카드의 수")]
    [SerializeField] private int m_minimum_count;
    public int Min => m_minimum_count;

    [Header("조건의 최대 카드의 수")]
    [SerializeField] private int m_maximum_count;
    public int Max => m_maximum_count;

    [Space(30f), Header("효과")]
    [Header("동시에 보유할 수 있는 카드의 수")]
    [SerializeField] private int m_hand_count;
    public int MaxHandCount => m_hand_count;

    [Header("한 턴에 사용할 수 있는 카드의 수")]
    [SerializeField] private int m_use_count;
    public int MaxUseCount => m_use_count;
}
