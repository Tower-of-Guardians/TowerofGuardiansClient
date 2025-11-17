using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagerInjector : MonoBehaviour, IInjector
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlayerUnit player;
    [SerializeField] private List<MonsterUnit> initialMonsters = new();
    [SerializeField] private Button attackButton;

    public void Inject()
    {
        if (battleManager == null)
        {
            battleManager = GetComponent<BattleManager>();
        }

        if (battleManager == null)
        {
            Debug.LogError("BattleManagerInjector requires a BattleManager reference.");
            return;
        }

        battleManager.Initialize(player, initialMonsters, attackButton);

        if (!DIContainer.IsRegistered<BattleManager>())
        {
            DIContainer.Register<BattleManager>(battleManager);
        }
    }
}

