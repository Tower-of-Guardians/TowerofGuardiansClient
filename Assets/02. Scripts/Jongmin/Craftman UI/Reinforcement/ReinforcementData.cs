using UnityEngine;

[System.Serializable]
public class ReinforcementData
{
    [Header("스테이지")]
    [SerializeField] private int m_stage;
    public int Stage => m_stage;

    [Header("강화 공격력 수치")]
    [SerializeField] private float m_atk;
    public float ATK => m_atk;

    [Header("강화 방어력 수치")]
    [SerializeField] private float m_def;
    public float DEF => m_def;

}