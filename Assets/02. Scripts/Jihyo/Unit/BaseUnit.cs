using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUnit : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected bool hasDefense;

    [Header("Status UI")]
    [SerializeField] protected TMP_Text attackText;
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected TMP_Text hpText;
    [SerializeField] protected Image hpFillImage;
    [SerializeField] protected GameObject defenseIcon;
    [SerializeField] protected Color defaultHpColor = Color.red;
    [SerializeField] protected Color defenseHpColor = Color.white;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    public bool HasDefense => hasDefense;

    protected virtual void Awake()
    {
        ClampHealth(forceMaxIfZero: true);
        RefreshUI();
    }

    public virtual void SetCurrentHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        RefreshUI();
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, maxHealth);
        RefreshUI();
    }

    public virtual void SetDefense(bool active)
    {
        hasDefense = active;
        RefreshUI();
    }

    protected virtual void ClampHealth(bool forceMaxIfZero = false)
    {
        if (forceMaxIfZero && currentHealth == 0)
        {
            currentHealth = maxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    protected virtual void RefreshUI()
    {
        float ratio = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

        if (hpSlider != null)
        {
            hpSlider.normalizedValue = ratio;
        }

        if (hpText != null)
        {
            hpText.text = $"HP {currentHealth}/{maxHealth}";
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

