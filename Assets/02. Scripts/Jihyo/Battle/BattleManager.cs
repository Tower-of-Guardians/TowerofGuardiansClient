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
    [SerializeField] private BattleTurnEndController turnEndController;
    [SerializeField] private BattleCombatController combatController;

    private bool isInitialized;
    
    [Header("Combat Pipeline")]
    private TurnPipeline combatAttackPipeline;
    private ElementContext elementContext;
    private bool isProcessingAttack;

    private void Awake()
    {
        InitializeControllers();
    }

    private void OnDestroy()
    {
        CleanupControllers();
    }

    private void InitializeControllers()
    {
        if (setupController != null)
        {
            setupController.Initialize(this);
        }
        if (actionController != null)
        {
            actionController.Initialize(this);
        }
        if (turnEndController != null)
        {
            turnEndController.Initialize(this);
        }
        if (combatController != null)
        {
            combatController.Initialize(this);
        }
    }

    private void CleanupControllers()
    {
        if (setupController != null)
        {
            setupController.Cleanup();
        }
        if (actionController != null)
        {
            actionController.Cleanup();
        }
        if (turnEndController != null)
        {
            turnEndController.Cleanup();
        }
        if (combatController != null)
        {
            combatController.Cleanup();
        }
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

        InitializePipeline();
        StartCoroutine(StartFirstTurnDelayed());
    }

    private void InitializePipeline()
    {
        // ElementContext 생성 및 의존성 주입
        elementContext = new ElementContext();
        
        // DI Container에서 의존성 가져오기 (등록될 때까지 대기 필요)
        StartCoroutine(InitializePipelineDelayed());
    }

    private IEnumerator InitializePipelineDelayed()
    {
        // 필요한 의존성들이 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<TurnManager>());
        yield return new WaitUntil(() => DIContainer.IsRegistered<HandPresenter>());
        yield return new WaitUntil(() => DIContainer.IsRegistered<AttackFieldPresenter>());

        // ElementContext에 의존성 주입
        elementContext.turn_manager = DIContainer.Resolve<TurnManager>();
        elementContext.hand_presenter = DIContainer.Resolve<HandPresenter>();
        elementContext.field_presenter = DIContainer.Resolve<AttackFieldPresenter>();
        elementContext.battle_action_controller = actionController;
        elementContext.setup_controller = setupController;
        elementContext.battle_manager = this; // 코루틴 실행 및 기타 기능을 위한 참조

        // 전투 공격 파이프라인 생성 및 파이프라인 요소 등록
        combatAttackPipeline = new TurnPipeline();
        
        // BattleCombatController에서 설정값 가져오기
        bool playerAttackHitsAll = combatController != null ? combatController.GetPlayerAttackHitsAll() : false;
        float statAnimationWaitTime = combatController != null ? combatController.GetStatAnimationWaitTime() : 1.0f;
        
        // AttackSequence를 파이프라인 요소들로 등록
        combatAttackPipeline.Register(new CombatInitializationElement(playerAttackHitsAll));
        combatAttackPipeline.Register(new PlayerAttackCalculationElement(statAnimationWaitTime));
        combatAttackPipeline.Register(new PlayerEnforceAnimationElement());
        combatAttackPipeline.Register(new PlayerDefenseEffectElement());
        combatAttackPipeline.Register(new PlayerMoveToAttackElement());
        combatAttackPipeline.Register(new PlayerAttackTriggerElement());
        combatAttackPipeline.Register(new RemoveDeadMonstersElement()); // 플레이어 공격 후 죽은 몬스터 제거
        combatAttackPipeline.Register(new VictoryCheckElement(this)); // 플레이어 공격 후 승리 체크
        combatAttackPipeline.Register(new MonsterAttackSequenceElement(this)); // 몬스터 공격 (내부에서 RemoveDeadMonstersElement와 승리 체크 포함)
        combatAttackPipeline.Register(new TurnEndRequestElement(this)); // 턴 종료 요청
    }

    private IEnumerator StartFirstTurnDelayed()
    {
        yield return new WaitUntil(() => DIContainer.IsRegistered<TurnManager>());

        var turnManager = DIContainer.Resolve<TurnManager>();
        if (turnManager != null)
        {
            turnManager.Initialize();
            turnManager.ResetTurnNumber();
            turnManager.StartTurn();
        }
    }

    public void OnAttackButtonClicked()
    {
        if (isProcessingAttack) return;

        if (combatAttackPipeline == null || elementContext == null) return;

        // 턴 시작 처리
        if (actionController != null)
        {
            actionController.OnTurnStart();
        }

        isProcessingAttack = true;
        combatAttackPipeline.Execute(elementContext, OnCombatAttackPipelineComplete);
    }

    private void OnCombatAttackPipelineComplete()
    {
        isProcessingAttack = false;
        
        // 공격 완료 후 처리
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

        // 공격 종료 시 턴 증가
        if (DIContainer.IsRegistered<TurnManager>())
        {
            var turnManager = DIContainer.Resolve<TurnManager>();
            if (turnManager != null)
            {
                turnManager.EndTurn();
                turnManager.StartTurn();
            }
        }
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

    public BattleSetupController GetSetupController()
    {
        return setupController;
    }

    public bool IsProcessingAttack()
    {
        return isProcessingAttack;
    }
}
