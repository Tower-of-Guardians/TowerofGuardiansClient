using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: PlayerStat, PlayerController, PlayerAnimation으로 분리, Player가 통합 관리
public class PlayerUnit : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    // TODO: 외부데이터로 받아올 예정
    [SerializeField] private const int MaxHealthConst = 100;
    [SerializeField] private int attack = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool hasDefense;

    [Header("Attack Thresholds")]
    [SerializeField] private const int lightAttack = 10;   // Attack1
    [SerializeField] private const int normalAttack = 20;   // Attack2

    [Header("Status UI")]
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private GameObject defenseIcon;
    [SerializeField] private Color defaultHpColor = Color.red;
    [SerializeField] private Color defenseHpColor = Color.white;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attack1TriggerName = "Attack1";
    [SerializeField] private string attack1StateName = "Player1_Attack1";
    [SerializeField] private string attack2TriggerName = "Attack2";
    [SerializeField] private string attack2StateName = "Player1_Attack2";
    [SerializeField] private string attack3TriggerName = "Attack3";
    [SerializeField] private string attack3StateName = "Player1_Attack3";

    [Header("Sprite")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float singleTargetAttackOffset = 1.0f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    [SerializeField] private float attackMotionDuration = 0.3f;
    [SerializeField] private float returnMoveDuration = 0.2f;

    private Vector3 initialSpriteLocalPosition;
    private bool hasCachedSpriteOrigin;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => MaxHealthConst;
    public int AttackValue => attack;
    public bool IsAlive => currentHealth > 0;
    public bool HasDefense => hasDefense;

    private void Awake()
    {
        InitializeAnimator();
        CacheSpriteOrigin();
        ClampHealth(forceMaxIfZero: true);
        RefreshUI();
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

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, amount), 0, MaxHealthConst);
        RefreshUI();
    }

    public IEnumerator PerformAttack(IEnumerable<IDamageable> targets, bool isAreaAttack = false, Vector3? primaryTargetWorldPosition = null)
    {
        CacheSpriteOrigin();

        if (spriteTransform != null)
        {
            Vector3 attackPosition = initialSpriteLocalPosition;

            if (isAreaAttack)
            {
                attackPosition.x = 0f;
            }
            else if (primaryTargetWorldPosition.HasValue)
            {
                Transform parent = spriteTransform.parent;
                Vector3 targetLocal = parent != null
                    ? parent.InverseTransformPoint(primaryTargetWorldPosition.Value)
                    : primaryTargetWorldPosition.Value;

                attackPosition.x = targetLocal.x - Mathf.Abs(singleTargetAttackOffset);
            }
            else
            {
                attackPosition.x = initialSpriteLocalPosition.x - Mathf.Abs(singleTargetAttackOffset);
            }

            // 공격 위치로 이동
            yield return StartCoroutine(MoveSpriteToPosition(attackPosition, attackMoveDuration));
        }

        // 공격 애니메이션 재생
        PlayAttackAnimation();

        // 공격 실행
        if (targets != null)
        {
            foreach (IDamageable target in targets)
            {
                if (target != null && target.IsAlive)
                {
                    target.TakeDamage(attack);
                }
            }
        }

        // 공격 애니메이션이 끝날 때까지 대기
        yield return StartCoroutine(WaitForAttackAnimationComplete());

        // 제자리로 복귀
        if (spriteTransform != null)
        {
            yield return StartCoroutine(MoveSpriteToPosition(initialSpriteLocalPosition, returnMoveDuration));
        }
    }

    private IEnumerator MoveSpriteToPosition(Vector3 targetPosition, float duration)
    {
        if (spriteTransform == null || duration <= 0f)
        {
            if (spriteTransform != null)
            {
                spriteTransform.localPosition = targetPosition;
            }
            yield break;
        }

        Vector3 startPosition = spriteTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            spriteTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        spriteTransform.localPosition = targetPosition;
    }

    public void SetDefense(bool active)
    {
        hasDefense = active;
        RefreshUI();
    }


    private void InitializeAnimator()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void PlayAttackAnimation()
    {
        if (animator == null)
        {
            return;
        }

        string triggerName = GetAttackTriggerName();
        if (!string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }
    }

    private string GetAttackTriggerName()
    {
        if (attack < lightAttack)
        {
            return attack1TriggerName;
        }
        else if (attack < normalAttack)
        {
            return attack2TriggerName;
        }
        else
        {
            return attack3TriggerName;
        }
    }

    private string GetAttackStateName()
    {
        if (attack < lightAttack)
        {
            return attack1StateName;
        }
        else if (attack < normalAttack)
        {
            return attack2StateName;
        }
        else
        {
            return attack3StateName;
        }
    }

    private IEnumerator WaitForAttackAnimationComplete()
    {
        string stateName = GetAttackStateName();
        
        if (animator == null || string.IsNullOrEmpty(stateName))
        {
            if (attackMotionDuration > 0f)
            {
                yield return new WaitForSeconds(attackMotionDuration);
            }
            yield break;
        }

        int attackStateHash = Animator.StringToHash(stateName);
        
        // Attack 상태로 전환될 때까지 대기
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.shortNameHash == attackStateHash)
            {
                break;
            }
            
            yield return null;
        }
        
        // Attack 상태가 끝날 때까지 대기
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.shortNameHash != attackStateHash)
            {
                break;
            }
            
            if (stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            
            yield return null;
        }
    }

    private void CacheSpriteOrigin()
    {
        if (spriteTransform != null && !hasCachedSpriteOrigin)
        {
            initialSpriteLocalPosition = spriteTransform.localPosition;
            hasCachedSpriteOrigin = true;
        }
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
            attackText.text = attack.ToString();
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