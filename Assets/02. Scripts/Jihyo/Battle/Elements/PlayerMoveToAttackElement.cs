using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어를 공격 위치로 이동시키는 파이프라인 요소
/// </summary>
public class PlayerMoveToAttackElement : IPipelineElement
{
    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null || context.combat_state == null)
        {
            Debug.LogError("PlayerMoveToAttackElement: setup_controller 또는 combat_state가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        if (player == null)
        {
            Debug.LogError("PlayerMoveToAttackElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        // TODO: 애니메이션 콜백으로 전환 필요
        // 현재는 코루틴으로 대기
        if (context.battle_manager != null)
        {
            context.battle_manager.StartCoroutine(
                MoveToAttackPosition(player, context.combat_state.attack_anchor_position, 
                    context.combat_state.player_attack_hits_all, onComplete)
            );
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator MoveToAttackPosition(Player player, Vector3? attackAnchorPosition, 
        bool isAreaAttack, Action onComplete)
    {
        yield return player.MoveToAttackPosition(attackAnchorPosition, isAreaAttack);
        onComplete?.Invoke();
    }
}
