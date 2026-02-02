using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 죽은 몬스터들을 제거하는 파이프라인 요소
/// </summary>
public class RemoveDeadMonstersElement : IPipelineElement
{
    public void execute(ElementContext context, Action onComplete)
    {
        if (context.setup_controller == null)
        {
            Debug.LogError("RemoveDeadMonstersElement: setup_controller가 null입니다.");
            onComplete?.Invoke();
            return;
        }

        var primaryMonsters = context.setup_controller.GetPrimaryMonsters();
        var monstersToRemove = primaryMonsters.Where(m => m != null && !m.IsAlive).ToList();

        foreach (Monster monster in monstersToRemove)
        {
            if (monster != null)
            {
                monster.DestroyMonster();
                context.setup_controller.UnregisterMonster(monster);
            }
        }

        onComplete?.Invoke();
    }
}
