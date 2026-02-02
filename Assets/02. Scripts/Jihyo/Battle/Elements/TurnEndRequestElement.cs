using System;

/// <summary>
/// 턴 종료를 요청하는 파이프라인 요소
/// </summary>
public class TurnEndRequestElement : IPipelineElement
{
    private readonly BattleManager battleManager;

    public TurnEndRequestElement(BattleManager battleManager)
    {
        this.battleManager = battleManager;
    }

    public void execute(ElementContext context, Action onComplete)
    {
        if (battleManager != null)
        {
            battleManager.RequestTurnEnd();
        }

        onComplete?.Invoke();
    }
}
