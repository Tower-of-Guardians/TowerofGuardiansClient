using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 승리 조건을 체크하는 파이프라인 요소
/// </summary>
public class VictoryCheckElement : IPipelineElement
{
    private readonly BattleManager battleManager;

    public VictoryCheckElement(BattleManager battleManager)
    {
        this.battleManager = battleManager;
    }

    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            Debug.LogError("VictoryCheckElement: setup_controller가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var primaryMonsters = context.setup_controller.GetPrimaryMonsters();
        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();

        if (aliveMonsters.Count == 0)
        {
            // 승리 처리
            if (battleManager != null && context.battle_manager != null)
            {
                context.battle_manager.StartCoroutine(
                    HandleVictory(battleManager, onComplete)
                );
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        else
        {
            // 승리하지 않았으면 다음 요소로 진행
            onComplete?.Invoke();
        }
    }

    private IEnumerator HandleVictory(BattleManager battleManager, Action onComplete)
    {
        yield return battleManager.HandleVictory();
        onComplete?.Invoke();
    }
}
