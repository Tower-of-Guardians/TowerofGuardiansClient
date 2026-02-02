using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 방어력 이펙트를 적용하는 파이프라인 요소
/// </summary>
public class PlayerDefenseEffectElement : IPipelineElement
{
    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            Debug.LogError("PlayerDefenseEffectElement: setup_controller가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        if (player == null)
        {
            Debug.LogError("PlayerDefenseEffectElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        // TODO: 애니메이션 콜백으로 전환 필요
        // 현재는 코루틴으로 대기
        if (context.battle_manager != null)
        {
            context.battle_manager.StartCoroutine(
                ApplyDefenseStatsWithEffect(player, onComplete)
            );
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator ApplyDefenseStatsWithEffect(Player player, Action onComplete)
    {
        yield return player.ApplyDefenseStatsWithEffect();
        onComplete?.Invoke();
    }
}
