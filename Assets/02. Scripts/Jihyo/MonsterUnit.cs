using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonsterUnit : MonoBehaviour, IDamageable, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] private MonsterData data;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool hasDefense;
    [SerializeField] private int defaultAttackValue = 0;

    [Header("Status UI")]
    [SerializeField] private Transform attackAnchor;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private GameObject defenseIcon;
    [SerializeField] private Color defaultHpColor = Color.red;
    [SerializeField] private Color defenseHpColor = Color.white;
    [SerializeField] private GameObject targetIndicator;

    public MonsterData Data => data;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => data != null ? data.HP : 0;
    public bool IsAlive => currentHealth > 0;
    public bool HasDefense => hasDefense;
    public event Action<MonsterUnit> Clicked;
    private BattleManager battleManager;
    private Coroutine registrationRoutine;

    private void Awake()
    {
        InitializeData();
        ClampHealth(forceMaxIfZero: true);
        RefreshUI();
        SetTargeted(false);
        RegisterBattleManager();
    }

    private void OnEnable()
    {
        RegisterBattleManager();
    }

    private void OnDisable()
    {
        if (battleManager != null)
        {
            battleManager.UnregisterMonster(this);
            battleManager = null;
        }

        if (registrationRoutine != null)
        {
            StopCoroutine(registrationRoutine);
            registrationRoutine = null;
        }
    }

    public void SetCurrentHealth(int value)
    {
        InitializeData();
        currentHealth = Mathf.Clamp(value, 0, data.HP);
        RefreshUI();
    }

    public void TakeDamage(int amount)
    {
        InitializeData();
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, data.HP);
        RefreshUI();
    }

    public void SetDefense(bool active)
    {
        hasDefense = active;
        RefreshUI();
    }

    public void SetData(MonsterData newData)
    {
        data = newData ?? new MonsterData();
        ClampHealth(forceMaxIfZero: true);
        RefreshUI();
    }

    public void SetTargeted(bool isTargeted)
    {
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(isTargeted);
        }
    }

    public Transform AttackAnchor => attackAnchor != null ? attackAnchor : transform;

    public int GetAttackValue()
    {
        InitializeData();

        if (data.Actions != null && data.Actions.Count > 0)
        {
            MonsterActionDefinition action = data.Actions.FirstOrDefault();
            if (action != null && action.Value > 0)
            {
                return action.Value;
            }
        }

        return defaultAttackValue;
    }

    public void PerformAttack(IDamageable target)
    {
        if (target == null || !target.IsAlive)
        {
            return;
        }

        int damage = GetAttackValue();
        if (damage > 0)
        {
            target.TakeDamage(damage);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsAlive)
        {
            return;
        }

        Clicked?.Invoke(this);
    }

    private void OnMouseDown()
    {
        if (!IsAlive)
        {
            return;
        }

        Clicked?.Invoke(this);
    }

    private void InitializeData()
    {
        if (data == null)
        {
            data = new MonsterData();
        }
    }

    private void RegisterBattleManager()
    {
        if (battleManager != null || registrationRoutine != null)
        {
            return;
        }

        if (DIContainer.IsRegistered<BattleManager>())
        {
            battleManager = DIContainer.Resolve<BattleManager>();
            battleManager.RegisterMonster(this);
        }
        else
        {
            registrationRoutine = StartCoroutine(WaitForBattleManager());
        }
    }

    private IEnumerator WaitForBattleManager()
    {
        while (!DIContainer.IsRegistered<BattleManager>())
        {
            yield return null;
        }

        registrationRoutine = null;
        battleManager = DIContainer.Resolve<BattleManager>();
        battleManager.RegisterMonster(this);
    }

    private void ClampHealth(bool forceMaxIfZero = false)
    {
        if (forceMaxIfZero && currentHealth == 0)
        {
            currentHealth = data.HP;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, data.HP);
    }

    private void RefreshUI()
    {
        if (data == null)
        {
            return;
        }

        int attackValue = defaultAttackValue;
        if (data.Actions != null && data.Actions.Count > 0)
        {
            attackValue = data.Actions.First().Value;
        }

        if (attackText != null)
        {
            attackText.text = attackValue.ToString();
        }

        float ratio = data.HP > 0 ? (float)currentHealth / data.HP : 0f;

        if (hpSlider != null)
        {
            hpSlider.normalizedValue = ratio;
        }

        if (hpText != null)
        {
            hpText.text = $"HP {currentHealth}/{data.HP}";
        }

        if (hpFillImage != null)
        {
            hpFillImage.color = hasDefense ? defenseHpColor : defaultHpColor;
        }

        if (defenseIcon != null)
        {
            defenseIcon.SetActive(hasDefense);
        }
    }
}