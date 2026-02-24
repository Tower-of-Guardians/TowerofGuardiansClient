using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleCombatController : MonoBehaviour, IBattleController
{
    [SerializeField] private bool playerAttackHitsAll;
    [SerializeField] private float statAnimationWaitTime = 1.0f;

    private BattleManager battleManager;
    private bool isInitialized;

    public float GetStatAnimationWaitTime() => statAnimationWaitTime;
    public bool GetPlayerAttackHitsAll() => playerAttackHitsAll;
    public bool IsInitialized => isInitialized;

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
        battleManager = null;
        isInitialized = false;
    }

    /// <summary>
    /// 전투 초기화 및 타겟 선택
    /// </summary>
    public CombatInitializationResult InitializeCombat(BattleSetupController setupController)
    {
        if (setupController == null)
        {
            Debug.LogError("BattleCombatController: setupController가 null입니다.");
            return null;
        }

        var player = setupController.GetPlayer();
        var primaryMonsters = setupController.GetPrimaryMonsters();

        if (player == null)
        {
            Debug.LogWarning("BattleCombatController: Player가 null입니다.");
            return null;
        }

        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("BattleCombatController: 공격할 몬스터가 없습니다.");
            return null;
        }

        // 타겟 선택
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
            Monster target = selectedTarget != null && selectedTarget.IsAlive 
                ? selectedTarget 
                : aliveMonsters[Random.Range(0, aliveMonsters.Count)];
            primaryMonsterTarget = target;
            playerTargets.Add(target);
        }

        Vector3? attackAnchorPosition = primaryMonsterTarget != null 
            ? primaryMonsterTarget.AttackAnchor.position 
            : null;

        // 애니메이션 리셋
        var playerAnimation = player.GetComponent<PlayerAnimation>();
        if (playerAnimation != null)
        {
            playerAnimation.ResetAnimationState();
        }

        return new CombatInitializationResult
        {
            player = player,
            playerTargets = playerTargets,
            primaryMonsterTarget = primaryMonsterTarget,
            attackAnchorPosition = attackAnchorPosition,
            playerAnimation = playerAnimation
        };
    }

    /// <summary>
    /// 플레이어 공격력 계산 및 적용
    /// </summary>
    public int CalculatePlayerAttack(Player player)
    {
        if (player == null) return 0;

        player.ApplyAttackStats();
        return player.AttackValue;
    }

    /// <summary>
    /// 플레이어 강화 애니메이션 재생
    /// </summary>
    public IEnumerator PlayEnforceAnimation(PlayerAnimation playerAnimation, int attackValue)
    {
        if (playerAnimation == null) yield break;

        playerAnimation.TriggerAttackByValue(attackValue);
        yield return playerAnimation.WaitForEnforceAnimationComplete(attackValue);
    }

    /// <summary>
    /// 플레이어 방어력 이펙트 적용
    /// </summary>
    public IEnumerator ApplyDefenseEffect(Player player)
    {
        if (player == null) yield break;
        yield return player.ApplyDefenseStatsWithEffect();
    }

    /// <summary>
    /// 플레이어를 공격 위치로 이동
    /// </summary>
    public IEnumerator MovePlayerToAttackPosition(Player player, Vector3? attackAnchorPosition, bool isAreaAttack)
    {
        if (player == null) yield break;
        yield return player.MoveToAttackPosition(attackAnchorPosition, isAreaAttack);
    }

    /// <summary>
    /// 플레이어 공격 트리거 및 데미지 적용
    /// </summary>
    public IEnumerator ExecutePlayerAttack(Player player, PlayerAnimation playerAnimation, 
        int currentAttack, List<IDamageable> targets)
    {
        if (player == null || targets == null) yield break;

        // Attack 트리거 발동
        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
        }

        // 공격 애니메이션 대기 후 데미지 적용
        float waitTime = currentAttack < 10 ? 1.0f : 0.8f;
        yield return new WaitForSeconds(waitTime);

        // 데미지 적용
        foreach (IDamageable target in targets)
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(currentAttack);
            }
        }

        // 공격 애니메이션 완료 대기
        if (playerAnimation != null)
        {
            yield return playerAnimation.WaitForAttackAnimationComplete(currentAttack);
        }

        // 0.5초 대기 후 제자리로 복귀
        yield return new WaitForSeconds(0.5f);
        yield return player.ReturnToOriginalPosition();

        // 애니메이션 종료 후 트리거 취소하여 BaseLayer로 복귀
        if (playerAnimation != null)
        {
            playerAnimation.ResetAnimationState();
        }
    }

    /// <summary>
    /// 몬스터 공격 시퀀스 실행
    /// </summary>
    public IEnumerator ExecuteMonsterAttackSequence(BattleSetupController setupController)
    {
        if (setupController == null) yield break;

        var player = setupController.GetPlayer();
        var primaryMonsters = setupController.GetPrimaryMonsters();

        if (player == null) yield break;

        // 몬스터 공격 대기
        yield return new WaitForSeconds(0.5f);

        // 타겟 선택 해제
        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }
        setupController.ClearSelectedTarget();

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
                yield break;
            }
        }
    }

    /// <summary>
    /// 승리 체크
    /// </summary>
    public bool CheckVictory(BattleSetupController setupController)
    {
        if (setupController == null) return false;

        var primaryMonsters = setupController.GetPrimaryMonsters();
        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        return aliveMonsters.Count == 0;
    }
}

/// <summary>
/// 전투 초기화 결과
/// </summary>
public class CombatInitializationResult
{
    public Player player { get; set; }
    public List<IDamageable> playerTargets { get; set; }
    public Monster primaryMonsterTarget { get; set; }
    public Vector3? attackAnchorPosition { get; set; }
    public PlayerAnimation playerAnimation { get; set; }
}

