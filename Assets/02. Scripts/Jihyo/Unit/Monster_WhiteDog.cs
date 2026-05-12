using UnityEngine;

/// <summary>
/// 흰둥이 전용 행동 패턴: 공격 -> 보호 -> 상태부여(약점노출 1스택) 순환
/// </summary>
public class Monster_WhiteDog : Monster
{
    [Header("흰둥이 데이터 ID")]
    [SerializeField] private string monsterId = "41001000";

    [Header("흰둥이 전용 수치")]
    [SerializeField] private int weaknessStack = 1;

    protected override void Awake()
    {
        SetMonsterDataId(monsterId);
        base.Awake();
    }

    protected override void ConfigureMonsterTraits()
    {
        int attackMin = 7;
        int attackMax = 8;
        int guardMin = 10;
        int guardMax = 11;
        int weakness = weaknessStack;
        string weaknessStatusId = StatusEffectController.WeaknessExposureStatusId;

        if (TryGetLoadedMonsterData(out MonsterData data))
        {
            attackMin = data.ATKMin;
            attackMax = data.ATKMax;
            guardMin = data.DEFMin;
            guardMax = data.DEFMax;
            if (data.Value1 > 0)
            {
                weakness = data.Value1;
            }

            if (!string.IsNullOrEmpty(data.StatusEffect1ID))
            {
                weaknessStatusId = data.StatusEffect1ID;
            }
        }

        OverrideBehavior(
            MonsterActionPatternType.Cycle,
            new MonsterActionDefinition
            {
                ActionId = "WHITE_DOG_ATTACK",
                ActionType = MonsterActionType.Attack,
                TargetType = MonsterActionTargetType.Player,
                MinValue = attackMin,
                MaxValue = attackMax
            },
            new MonsterActionDefinition
            {
                ActionId = "WHITE_DOG_GUARD",
                ActionType = MonsterActionType.Guard,
                TargetType = MonsterActionTargetType.Self,
                MinValue = guardMin,
                MaxValue = guardMax
            },
            new MonsterActionDefinition
            {
                ActionId = "WHITE_DOG_WEAKNESS",
                ActionType = MonsterActionType.ApplyStatus,
                TargetType = MonsterActionTargetType.Player,
                StatusEffectId = weaknessStatusId,
                StatusStack = weakness
            }
        );
    }
}
