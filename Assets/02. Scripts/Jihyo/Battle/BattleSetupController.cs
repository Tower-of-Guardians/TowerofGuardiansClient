using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSetupController : MonoBehaviour, IBattleController
{
    private BattleManager battleManager;
    private bool isInitialized;

    private readonly List<Monster> primaryMonsters = new();
    private Player player;
    private Button attackButton;
    private Monster selectedTarget;

    public bool IsInitialized => isInitialized;

    public void Initialize(BattleManager manager)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleSetupController has already been initialized.");
            return;
        }

        battleManager = manager;
        isInitialized = true;
    }

    public void Cleanup()
    {
        DetachAttackButton();
        
        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.Clicked -= OnMonsterClicked;
            }
        }
        
        primaryMonsters.Clear();
        selectedTarget = null;
        player = null;
        attackButton = null;
        battleManager = null;
        isInitialized = false;
    }

    public void SetupBattle(Player playerUnit, IEnumerable<Monster> monsters, Button attackBtn)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("BattleSetupController is not initialized.");
            return;
        }

        player = playerUnit;
        attackButton = attackBtn;
        AttachAttackButton();

        if (monsters != null)
        {
            foreach (Monster monster in monsters)
            {
                RegisterMonster(monster);
            }
        }

        // 처음 카드 뽑기
        StartCoroutine(DrawCardsAtTurnStartDelayed());
    }

    public void RegisterMonster(Monster monster)
    {
        if (monster == null || primaryMonsters.Contains(monster))
        {
            return;
        }

        primaryMonsters.Add(monster);
        monster.Clicked += OnMonsterClicked;
    }

    public void UnregisterMonster(Monster monster)
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

    public void SetPlayer(Player playerUnit)
    {
        player = playerUnit;
    }

    public Monster GetSelectedTarget()
    {
        return selectedTarget;
    }

    public List<Monster> GetPrimaryMonsters()
    {
        return new List<Monster>(primaryMonsters);
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void ClearSelectedTarget()
    {
        if (selectedTarget != null)
        {
            selectedTarget.SetTargeted(false);
            selectedTarget = null;
        }
    }

    private void OnMonsterClicked(Monster monster)
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

    private void AttachAttackButton()
    {
        if (attackButton != null && battleManager != null)
        {
            attackButton.onClick.AddListener(battleManager.OnAttackButtonClicked);
        }
    }

    private void DetachAttackButton()
    {
        if (attackButton != null && battleManager != null)
        {
            attackButton.onClick.RemoveListener(battleManager.OnAttackButtonClicked);
        }
    }

    private IEnumerator DrawCardsAtTurnStartDelayed()
    {
        // HandPresenter와 TurnManager가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<HandPresenter>() && DIContainer.IsRegistered<TurnManager>());
        
        // 덱이 준비될 때까지 대기
        yield return new WaitUntil(() => GameData.Instance != null && GameData.Instance.notuseDeck.Count > 0);

        if (battleManager != null)
        {
            battleManager.RequestDrawCards();
        }
    }
}

