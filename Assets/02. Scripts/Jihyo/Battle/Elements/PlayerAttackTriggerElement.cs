using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 공격 트리거 및 데미지 적용을 수행하는 파이프라인 요소
/// </summary>
public class PlayerAttackTriggerElement : IPipelineElement
{
    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null || context.combat_state == null)
        {
            Debug.LogError("PlayerAttackTriggerElement: setup_controller 또는 combat_state가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        if (player == null)
        {
            Debug.LogError("PlayerAttackTriggerElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var playerAnimation = player.GetComponent<PlayerAnimation>();
        int currentAttack = context.combat_state.current_attack;

        // Attack 트리거 발동
        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
        }

        // TODO: 애니메이션 콜백으로 전환 필요
        // 현재는 코루틴으로 대기 후 데미지 적용
        if (context.battle_manager != null)
        {
            context.battle_manager.StartCoroutine(
                WaitAndApplyDamage(context, currentAttack, playerAnimation, onComplete)
            );
        }
        else
        {
            // 코루틴 없이 즉시 데미지 적용
            ApplyDamage(context, currentAttack);
            onComplete?.Invoke();
        }
    }

    private IEnumerator WaitAndApplyDamage(ElementContext context, int currentAttack, 
        PlayerAnimation playerAnimation, Action onComplete)
    {
        // 공격 애니메이션 대기 후 데미지 적용(하드코딩)
        float waitTime = currentAttack < 10 ? 1.0f : 0.8f;
        yield return new WaitForSeconds(waitTime);

        // 데미지 적용
        ApplyDamage(context, currentAttack);

        // 공격 애니메이션 완료 대기
        if (playerAnimation != null)
        {
            yield return playerAnimation.WaitForAttackAnimationComplete(currentAttack);
        }

        // 0.5초 대기 후 제자리로 복귀(하드코딩)
        yield return new WaitForSeconds(0.5f);
        
        var player = context.setup_controller.GetPlayer();
        if (player != null)
        {
            yield return player.ReturnToOriginalPosition();
        }

        // 애니메이션 종료 후 트리거 취소하여 BaseLayer로 복귀
        if (playerAnimation != null)
        {
            playerAnimation.ResetAnimationState();
        }

        onComplete?.Invoke();
    }

    private void ApplyDamage(ElementContext context, int currentAttack)
    {
        if (context.combat_state?.player_targets == null)
        {
            return;
        }

        foreach (IDamageable target in context.combat_state.player_targets)
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(currentAttack);
            }
        }
    }
}
