using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 강화 애니메이션을 재생하는 파이프라인 요소
/// </summary>
public class PlayerEnforceAnimationElement : IPipelineElement
{
    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null || context.combat_state == null)
        {
            Debug.LogError("PlayerEnforceAnimationElement: setup_controller 또는 combat_state가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        if (player == null)
        {
            Debug.LogError("PlayerEnforceAnimationElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var playerAnimation = player.GetComponent<PlayerAnimation>();
        if (playerAnimation == null)
        {
            Debug.LogWarning("PlayerEnforceAnimationElement: PlayerAnimation이 없습니다.");
            onComplete?.Invoke();
            return;
        }

        int currentAttack = context.combat_state.current_attack;
        playerAnimation.TriggerAttackByValue(currentAttack);

        // TODO: 애니메이션 콜백으로 전환 필요
        // 현재는 코루틴으로 대기
        if (context.battle_manager != null)
        {
            context.battle_manager.StartCoroutine(
                WaitForEnforceAnimation(playerAnimation, currentAttack, onComplete)
            );
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator WaitForEnforceAnimation(PlayerAnimation playerAnimation, int attackValue, Action onComplete)
    {
        yield return playerAnimation.WaitForEnforceAnimationComplete(attackValue);
        onComplete?.Invoke();
    }
}
