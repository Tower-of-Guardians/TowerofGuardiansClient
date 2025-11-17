using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private bool playerAttackHitsAll;
    [SerializeField] private float monsterAttackDelay = 0.15f;

    private readonly List<MonsterUnit> primaryMonsters = new();
    private PlayerUnit player;
    private Button attackButton;
    private MonsterUnit selectedTarget;
    private bool isProcessingAttack;
    private bool isInitialized;

    private void OnDestroy()
    {
        DetachAttackButton();
        foreach (MonsterUnit monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.Clicked -= OnMonsterClicked;
            }
        }
        primaryMonsters.Clear();
        selectedTarget = null;
    }

    private void OnMonsterClicked(MonsterUnit monster)
    {
        if (monster == null || !monster.IsAlive)
        {
            return;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
            return;
        }

        if (selectedTarget != null)
        {
            selectedTarget.SetTargeted(false);
        }

        selectedTarget = monster;
        selectedTarget.SetTargeted(true);
    }

    public void Initialize(PlayerUnit playerUnit, IEnumerable<MonsterUnit> monsters, Button attackBtn)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleManager has already been initialized.");
            return;
        }

        player = playerUnit;
        attackButton = attackBtn;
        AttachAttackButton();

        if (monsters != null)
        {
            foreach (MonsterUnit monster in monsters)
            {
                RegisterMonster(monster);
            }
        }

        isInitialized = true;
    }

    public void RegisterMonster(MonsterUnit monster)
    {
        if (monster == null || primaryMonsters.Contains(monster))
        {
            return;
        }

        primaryMonsters.Add(monster);
        monster.Clicked += OnMonsterClicked;
    }

    public void UnregisterMonster(MonsterUnit monster)
    {
        if (monster == null)
        {
            return;
        }

        if (primaryMonsters.Remove(monster))
        {
            monster.Clicked -= OnMonsterClicked;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
        }
    }

    public void ConfigureAttackButton(Button button)
    {
        if (attackButton == button)
        {
            return;
        }

        DetachAttackButton();
        attackButton = button;
        AttachAttackButton();
    }

    public void SetPlayer(PlayerUnit playerUnit)
    {
        player = playerUnit;
    }

    private void HandleAttackButton()
    {
        if (isProcessingAttack || player == null)
        {
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isProcessingAttack = true;

        List<MonsterUnit> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("No monsters available to attack.");
            isProcessingAttack = false;
            yield break;
        }

        List<IDamageable> playerTargets = new();
        MonsterUnit primaryMonsterTarget = null;
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
            MonsterUnit target = selectedTarget != null && selectedTarget.IsAlive
                ? selectedTarget
                : aliveMonsters[Random.Range(0, aliveMonsters.Count)];

            primaryMonsterTarget = target;
            playerTargets.Add(target);
        }

        Vector3? attackAnchorPosition = primaryMonsterTarget != null
            ? primaryMonsterTarget.AttackAnchor.position
            : (Vector3?)null;

        yield return StartCoroutine(player.PerformAttack(playerTargets, playerAttackHitsAll, attackAnchorPosition));

        // 몬스터 공격 대기
        yield return new WaitForSeconds(0.5f);

        foreach (MonsterUnit monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }
        selectedTarget = null;

        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();

        foreach (MonsterUnit monster in aliveMonsters)
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
                break;
            }
        }

        isProcessingAttack = false;
    }

    private void AttachAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(HandleAttackButton);
        }
    }

    private void DetachAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveListener(HandleAttackButton);
        }
    }
}

