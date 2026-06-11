using UnityEngine;

/// <summary>
/// 클론. MonsterData(41001005) 공격 패턴만 사용합니다.
/// </summary>
public class Monster_ClonierClone : Monster
{
    [Header("클론 데이터 ID")]
    [SerializeField] private string monsterId = "41001005";

    protected override void Awake()
    {
        SetMonsterDataId(monsterId);
        base.Awake();
    }
}
