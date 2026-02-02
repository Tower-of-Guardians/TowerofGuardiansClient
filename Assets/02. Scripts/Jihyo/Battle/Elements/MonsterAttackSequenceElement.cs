using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 몬스터 공격 시퀀스를 처리하는 파이프라인 요소
/// </summary>
public class MonsterAttackSequenceElement : IPipelineElement
{
    private readonly BattleManager battleManager;

    public MonsterAttackSequenceElement(BattleManager battleManager)
    {
        this.battleManager = battleManager;
    }

    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            Debug.LogError("MonsterAttackSequenceElement: setup_controller가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var player = context.setup_controller.GetPlayer();
        var primaryMonsters = context.setup_controller.GetPrimaryMonsters();

        if (player == null)
        {
            Debug.LogError("MonsterAttackSequenceElement: Player가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        // TODO: 애니메이션 콜백으로 전환 필요
        // 현재는 코루틴으로 처리
        if (context.battle_manager != null)
        {
            context.battle_manager.StartCoroutine(
                ProcessMonsterAttacks(context, player, primaryMonsters, onComplete)
            );
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator ProcessMonsterAttacks(ElementContext context, Player player, 
        List<Monster> primaryMonsters, Action onComplete)
    {
        // 몬스터 공격 대기
        yield return new WaitForSeconds(0.5f);

        // 타겟 선택 해제
        ClearTargetSelection(context, primaryMonsters);

        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();

        foreach (Monster monster in aliveMonsters)
        {
            if (monster == null || !monster.IsAlive)
            {
                continue;
            }

            yield return monster.PerformAttack(player);

            if (!player.IsAlive)
            {
                Debug.Log("Player defeated.");
                if (battleManager != null)
                {
                    yield return battleManager.HandleDefeat();
                }
                onComplete?.Invoke();
                yield break;
            }
        }

        // 몬스터 공격 후 죽은 몬스터들 제거
        var removeDeadMonstersElement = new RemoveDeadMonstersElement();
        removeDeadMonstersElement.execute(context, () => { });

        // 최종 승리 체크
        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0 && battleManager != null)
        {
            yield return battleManager.HandleVictory();
            onComplete?.Invoke();
            yield break;
        }

        onComplete?.Invoke();
    }

    private void ClearTargetSelection(ElementContext context, List<Monster> monsters)
    {
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }

        if (context.setup_controller != null)
        {
            context.setup_controller.ClearSelectedTarget();
        }
    }
}
