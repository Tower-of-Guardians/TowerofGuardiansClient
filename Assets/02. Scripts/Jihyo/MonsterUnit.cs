using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonsterUnit : MonoBehaviour, IDamageable, IPointerClickHandler
{
    [Header("Data")]
    // TODO: 외부데이터로 받아올 예정
    [SerializeField] private const int MaxHealthConst = 100;
    [SerializeField] private const int Attack = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool hasDefense;

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

    public int CurrentHealth => currentHealth;
    public int MaxHealth => MaxHealthConst;
    public bool IsAlive => currentHealth > 0;
    public bool HasDefense => hasDefense;
    public event Action<MonsterUnit> Clicked;
    private BattleManager battleManager;
    private Coroutine registrationRoutine;

    private void Awake()
    {
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
        currentHealth = Mathf.Clamp(value, 0, MaxHealthConst);
        RefreshUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, MaxHealthConst);
        RefreshUI();
    }

    public void SetDefense(bool active)
    {
        hasDefense = active;
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
        return Attack;
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
        // UI 위에 포인터가 있으면 몬스터 클릭 무시
        if (IsPointerOverUI(eventData))
        {
            return;
        }

        if (!IsAlive)
        {
            return;
        }

        Clicked?.Invoke(this);
    }

    private void OnMouseDown()
    {
        // UI 위에 포인터가 있으면 몬스터 클릭 무시
        if (IsPointerOverUI())
        {
            return;
        }

        if (!IsAlive)
        {
            return;
        }

        Clicked?.Invoke(this);
    }

    /// <summary>
    /// 포인터가 UI 위에 있는지 확인 (PointerEventData 사용)
    /// </summary>
    private bool IsPointerOverUI(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // UI 요소가 있으면 true 반환
        return results.Count > 0;
    }

    /// <summary>
    /// 포인터가 UI 위에 있는지 확인
    /// </summary>
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (Input.touchCount > 0)
        {
            // 터치 입력
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        else
        {
            // 마우스 입력
            return EventSystem.current.IsPointerOverGameObject();
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
            currentHealth = MaxHealthConst;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealthConst);
    }

    private void RefreshUI()
    {
        if (attackText != null)
        {
            attackText.text = Attack.ToString();
        }

        float ratio = MaxHealthConst > 0 ? (float)currentHealth / MaxHealthConst : 0f;

        if (hpSlider != null)
        {
            hpSlider.normalizedValue = ratio;
        }

        if (hpText != null)
        {
            hpText.text = $"HP {currentHealth}/{MaxHealthConst}";
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