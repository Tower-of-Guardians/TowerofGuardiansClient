using UnityEngine;

/// <summary>
/// 흰둥이 전용 행동 패턴: 공격 -> 보호 -> 상태부여(약점노출 1스택) 순환
/// </summary>
public class Monster_WhiteDog : Monster
{
    [Header("흰둥이 전용 수치")]
    [SerializeField] private int attackMin = 7;
    [SerializeField] private int attackMax = 8;
    [SerializeField] private int guardMin = 10;
    [SerializeField] private int guardMax = 11;
    [SerializeField] private int weaknessStack = 1;

    protected override void ConfigureMonsterTraits()
    {
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
                StatusEffectId = StatusEffectController.WeaknessExposureStatusId,
                StatusStack = weaknessStack
            }
        );
    }
}
