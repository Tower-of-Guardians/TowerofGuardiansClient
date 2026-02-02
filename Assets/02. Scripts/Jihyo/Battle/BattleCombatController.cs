using UnityEngine;

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
}

