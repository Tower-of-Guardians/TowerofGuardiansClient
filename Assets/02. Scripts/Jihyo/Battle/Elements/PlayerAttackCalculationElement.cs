using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 공격력을 계산하고 적용하는 파이프라인 요소
/// </summary>
public class PlayerAttackCalculationElement : IPipelineElement
{
    private readonly float statAnimationWaitTime;

    public PlayerAttackCalculationElement(float statAnimationWaitTime)
    {
        this.statAnimationWaitTime = statAnimationWaitTime;
    }

    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        if (player == null)
        {
            onComplete?.Invoke();
            return;
        }

        // 공격력 적용 및 이펙트
        player.ApplyAttackStats();

        // 공격력 확인
        int currentAttack = player.AttackValue;
        
        // 상태에 저장
        if (context.combat_state == null)
        {
            context.combat_state = new CombatState();
        }
        context.combat_state.current_attack = currentAttack;

        // 공격력 애니메이션 대기
        if (context.battle_manager != null && statAnimationWaitTime > 0f)
        {
            context.battle_manager.StartCoroutine(
                WaitForStatAnimation(statAnimationWaitTime, onComplete)
            );
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator WaitForStatAnimation(float waitTime, Action onComplete)
    {
        yield return new WaitForSeconds(waitTime);
        onComplete?.Invoke();
    }
}
