using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private BattleSetupController setupController;
    [SerializeField] private BattleActionController actionController;
    [SerializeField] private BattleCombatController combatController;
    [SerializeField] private BattleTurnEndController turnEndController;

    private bool isInitialized;
    private readonly List<IBattleController> controllers = new();

    private void Awake()
    {
        InitializeControllers();
    }

    private void OnDestroy()
    {
        CleanupAllControllers();
    }

    private void InitializeControllers()
    {
        if (setupController != null)
        {
            controllers.Add(setupController);
        }
        if (actionController != null)
        {
            controllers.Add(actionController);
        }
        if (combatController != null)
        {
            controllers.Add(combatController);
        }
        if (turnEndController != null)
        {
            controllers.Add(turnEndController);
        }

        // 각 Controller 초기화
        foreach (var controller in controllers)
        {
            controller.Initialize(this);
        }

        // Controller 간 의존성 설정
        if (combatController != null && setupController != null)
        {
            combatController.SetSetupController(setupController);
        }
    }

    private void CleanupAllControllers()
    {
        foreach (var controller in controllers)
        {
            controller.Cleanup();
        }
        controllers.Clear();
    }

    public void Initialize(Player playerUnit, IEnumerable<Monster> monsters, Button attackBtn)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleManager has already been initialized.");
            return;
        }

        if (setupController == null)
        {
            Debug.LogError("BattleSetupController is not assigned.");
            return;
        }

        setupController.SetupBattle(playerUnit, monsters, attackBtn);
        isInitialized = true;
    }

    public void OnAttackButtonClicked()
    {
        if (combatController == null)
        {
            Debug.LogWarning("BattleCombatController is not assigned.");
            return;
        }

        if (combatController.IsProcessingAttack)
        {
            return;
        }

        // 턴 시작 처리
        if (actionController != null)
        {
            actionController.OnTurnStart();
        }

        // 공격 시퀀스 시작
        combatController.StartAttackSequence();
    }

    public void RequestDrawCards(int count = -1)
    {
        if (turnEndController == null)
        {
            Debug.LogWarning("BattleTurnEndController is not assigned.");
            return;
        }

        turnEndController.DrawCards(count);
    }

    public void RequestTurnEnd()
    {
        if (turnEndController == null)
        {
            Debug.LogWarning("BattleTurnEndController is not assigned.");
            return;
        }

        turnEndController.ProcessTurnEnd();
    }

    public IEnumerator HandleVictory()
    {
        // 보상 계산
        int totalGold = CalculateTotalGold();
        int totalExp = CalculateTotalExp();

        // ResultPresenter가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<ResultPresenter>());

        // Result 창 열기
        var resultPresenter = DIContainer.Resolve<ResultPresenter>();
        var resultData = new ResultData(BattleResultType.Victory, totalGold, totalExp);
        resultPresenter.OpenUI(resultData);
    }

    public IEnumerator HandleDefeat()
    {
        // ResultPresenter가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<ResultPresenter>());

        // Result 창 열기
        var resultPresenter = DIContainer.Resolve<ResultPresenter>();
        var resultData = new ResultData(BattleResultType.Defeat, 0, 0);
        resultPresenter.OpenUI(resultData);
    }

    private int CalculateTotalGold()
    {
        // TODO: 몬스터 데이터에서 골드 정보 가져오기
        if (setupController == null)
        {
            return 0;
        }

        var monsters = setupController.GetPrimaryMonsters();
        int totalGold = 0;
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                // 몬스터당 기본 골드 (나중에 몬스터 데이터에서 가져오도록 수정)
                totalGold += 100;
            }
        }
        return totalGold;
    }

    private int CalculateTotalExp()
    {
        // TODO: 몬스터 데이터에서 경험치 정보 가져오기
        if (setupController == null)
        {
            return 0;
        }

        var monsters = setupController.GetPrimaryMonsters();
        int totalExp = 0;
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                // 몬스터당 기본 경험치 (나중에 몬스터 데이터에서 가져오도록 수정)
                totalExp += 50;
            }
        }
        return totalExp;
    }

    public void RegisterMonster(Monster monster)
    {
        if (setupController != null)
        {
            setupController.RegisterMonster(monster);
        }
    }

    public void UnregisterMonster(Monster monster)
    {
        if (setupController != null)
        {
            setupController.UnregisterMonster(monster);
        }
    }

    public void ConfigureAttackButton(Button button)
    {
        if (setupController != null)
        {
            setupController.ConfigureAttackButton(button);
        }
    }

    public void SetPlayer(Player playerUnit)
    {
        if (setupController != null)
        {
            setupController.SetPlayer(playerUnit);
        }
    }
}
