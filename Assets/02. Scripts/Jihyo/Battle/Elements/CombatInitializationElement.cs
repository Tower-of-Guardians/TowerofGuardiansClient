using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 전투 초기화 및 타겟 선택을 수행하는 파이프라인 요소
/// </summary>
public class CombatInitializationElement : IPipelineElement
{
    private readonly bool playerAttackHitsAll;

    public CombatInitializationElement(bool playerAttackHitsAll)
    {
        this.playerAttackHitsAll = playerAttackHitsAll;
    }

    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            Debug.LogError("CombatInitializationElement: setup_controller가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        var primaryMonsters = context.setup_controller.GetPrimaryMonsters();

        if (player == null)
        {
            Debug.LogWarning("CombatInitializationElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("CombatInitializationElement: 공격할 몬스터가 없습니다.");
            onComplete?.Invoke();
            return;
        }

        // 타겟 선택
        List<IDamageable> playerTargets = new();
        Monster primaryMonsterTarget = null;
        Monster selectedTarget = context.setup_controller.GetSelectedTarget();

        if (playerAttackHitsAll)
        {
            playerTargets.AddRange(aliveMonsters);
            if (aliveMonsters.Count > 0)
            {
                primaryMonsterTarget = aliveMonsters[0];
            }
        }
        else
        {
            Monster target = selectedTarget != null && selectedTarget.IsAlive 
                ? selectedTarget 
                : aliveMonsters[Random.Range(0, aliveMonsters.Count)];
            primaryMonsterTarget = target;
            playerTargets.Add(target);
        }

        // 공격 앵커 위치 계산
        Vector3? attackAnchorPosition = primaryMonsterTarget != null 
            ? primaryMonsterTarget.AttackAnchor.position 
            : null;

        // 애니메이션 리셋
        var playerAnimation = player.GetComponent<PlayerAnimation>();
        if (playerAnimation != null)
        {
            playerAnimation.ResetAnimationState();
        }

        // 결과를 ElementContext에 저장
        if (context.combat_state == null)
        {
            context.combat_state = new CombatState();
        }
        context.combat_state.player_targets = playerTargets;
        context.combat_state.primary_monster_target = primaryMonsterTarget;
        context.combat_state.attack_anchor_position = attackAnchorPosition;
        context.combat_state.player_attack_hits_all = playerAttackHitsAll;

        onComplete?.Invoke();
    }
}
