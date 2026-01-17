using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleCombatController : MonoBehaviour, IBattleController
{
    [SerializeField] private bool playerAttackHitsAll;
    [SerializeField] private float monsterAttackDelay = 0.5f;
    [SerializeField] private float statAnimationWaitTime = 1.0f;

    private BattleManager battleManager;

    public float GetStatAnimationWaitTime() => statAnimationWaitTime;
    private BattleSetupController setupController;
    private bool isInitialized;
    private bool isProcessingAttack;

    public bool IsInitialized => isInitialized;
    public bool IsProcessingAttack => isProcessingAttack;

    public void Initialize(BattleManager manager)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleCombatController has already been initialized.");
            return;
        }

        battleManager = manager;
        isInitialized = true;
    }

    public void Cleanup()
    {
        isProcessingAttack = false;
        battleManager = null;
        setupController = null;
        isInitialized = false;
    }

    public void SetSetupController(BattleSetupController controller)
    {
        setupController = controller;
    }

    public void StartAttackSequence()
    {
        if (isProcessingAttack || setupController == null)
        {
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isProcessingAttack = true;

        if (setupController == null)
        {
            Debug.LogWarning("BattleSetupController is not set.");
            isProcessingAttack = false;
            yield break;
        }

        var player = setupController.GetPlayer();
        var primaryMonsters = setupController.GetPrimaryMonsters();

        if (player == null)
        {
            Debug.LogWarning("Player is null.");
            isProcessingAttack = false;
            yield break;
        }

        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("No monsters available to attack.");
            isProcessingAttack = false;
            yield break;
        }

        List<IDamageable> playerTargets = new();
        Monster primaryMonsterTarget = null;
        Monster selectedTarget = setupController.GetSelectedTarget();

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
            Monster target = selectedTarget != null && selectedTarget.IsAlive ? selectedTarget : aliveMonsters[Random.Range(0, aliveMonsters.Count)];
            primaryMonsterTarget = target;
            playerTargets.Add(target);
        }

        Vector3? attackAnchorPosition = primaryMonsterTarget != null ? primaryMonsterTarget.AttackAnchor.position : null;
        var playerAnimation = player.GetComponent<PlayerAnimation>();

        playerAnimation.ResetAnimationState();

        // TODO: 시너지 발동, 즉발 카드 효과 발동
        // yield return StartCoroutine(ActivateSynergyAndInstantCards());

        // 공격력 적용 및 이펙트
        player.ApplyAttackStats();
        yield return new WaitForSeconds(statAnimationWaitTime);

        // 공격력 확인
        int currentAttack = player.AttackValue;

        playerAnimation.TriggerAttackByValue(currentAttack);

        yield return StartCoroutine(playerAnimation.WaitForEnforceAnimationComplete(currentAttack));

        // 방어력 이펙트
        yield return StartCoroutine(player.ApplyDefenseStatsWithEffect());

        // 공격 위치로 이동
        yield return StartCoroutine(player.MoveToAttackPosition(attackAnchorPosition, playerAttackHitsAll));

        // Attack 트리거 발동
        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
        }

        // TODO: 공격 애니메이션 대기 후 데미지 적용(하드코딩)
        if (currentAttack < 10)
        {
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
        }

        // 공격 실행
        if (playerTargets != null)
        {
            foreach (IDamageable target in playerTargets)
            {
                if (target != null && target.IsAlive)
                {
                    target.TakeDamage(currentAttack);
                }
            }
        }

        // 공격 애니메이션 완료 대기
        if (playerAnimation != null)
        {
            yield return StartCoroutine(playerAnimation.WaitForAttackAnimationComplete(currentAttack));
        }

        // 0.5초 대기 후 제자리로 복귀(하드코딩)
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(player.ReturnToOriginalPosition());

        // 애니메이션 종료 후 트리거 취소하여 BaseLayer로 복귀
        if (playerAnimation != null)
        {
            playerAnimation.ResetAnimationState();
        }

        // 공격 후 발동 카드 효과 발동

        // 공격 완료 후 죽은 몬스터들 제거
        RemoveDeadMonsters(primaryMonsters);

        // 모든 몬스터 처치 확인
        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            isProcessingAttack = false;
            yield return StartCoroutine(battleManager.HandleVictory());
            yield break;
        }

        // 몬스터 공격 대기
        yield return new WaitForSeconds(0.5f);

        // 타겟 선택 해제
        ClearTargetSelection(primaryMonsters);

        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();

        foreach (Monster monster in aliveMonsters)
        {
            if (monster == null || !monster.IsAlive)
            {
                continue;
            }

            monster.PerformAttack(player);

            if (monsterAttackDelay > 0f)
            {
                yield return new WaitForSeconds(monsterAttackDelay);
            }

            if (!player.IsAlive)
            {
                Debug.Log("Player defeated.");
                isProcessingAttack = false;
                yield return StartCoroutine(battleManager.HandleDefeat());
                yield break;
            }
        }

        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            isProcessingAttack = false;
            yield return StartCoroutine(battleManager.HandleVictory());
            yield break;
        }

        // 몬스터 공격 후 죽은 몬스터들 제거
        RemoveDeadMonsters(primaryMonsters);

        isProcessingAttack = false;

        // 턴 종료 처리 요청
        if (battleManager != null)
        {
            battleManager.RequestTurnEnd();
        }
    }

    private void RemoveDeadMonsters(List<Monster> monsters)
    {
        var monstersToRemove = monsters.Where(m => m != null && !m.IsAlive).ToList();

        foreach (Monster monster in monstersToRemove)
        {
            if (monster != null)
            {
                monster.DestroyMonster();
                if (setupController != null)
                {
                    setupController.UnregisterMonster(monster);
                }
            }
        }
    }

    private void ClearTargetSelection(List<Monster> monsters)
    {
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }

        if (setupController != null)
        {
            setupController.ClearSelectedTarget();
        }
    }
}

