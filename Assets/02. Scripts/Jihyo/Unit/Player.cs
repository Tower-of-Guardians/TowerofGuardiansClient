using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : BaseUnit
{
    [Header("Stats")]
    private int baseAttack;
    private float cardAttackBonus;
    private float cardDefenseBonus;

    [Header("Animation")]
    private PlayerAnimation playerAnimation;

    [Header("Sprite")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float singleTargetAttackOffset = 3.0f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    [SerializeField] private float returnMoveDuration = 0.2f;

    private Vector3 initialSpriteLocalPosition;
    private bool hasCachedSpriteOrigin;

    public int AttackValue => Mathf.RoundToInt(baseAttack + cardAttackBonus);
    public float DefenseValue => cardDefenseBonus;

    protected override void Awake()
    {
        base.Awake();
        InitializeFromDataCenter();
        InitializeAnimation();
        CacheSpriteOrigin();
    }

    private void InitializeFromDataCenter()
    {
        if (DataCenter.Instance != null)
        {
            DataCenter.Instance.LoadPlayerData();
            var playerState = DataCenter.Instance.playerstate;
            
            baseAttack = playerState.atk;
            maxHealth = playerState.hp;
            
            if (currentHealth == 0)
            {
                currentHealth = maxHealth;
            }
            
            UpdateCardStats();
            RefreshUI();
        }
        else
        {
            Debug.LogWarning("Player: DataCenter.Instance is null. Using default values.");
            baseAttack = 5;
        }
    }

    public void UpdateCardStats()
    {
        if (GameData.Instance != null)
        {
            cardAttackBonus = GameData.Instance.AttackField();
            cardDefenseBonus = GameData.Instance.DefenseField();
            
            // 보호력이 있으면 방어 상태로 설정
            SetDefense(cardDefenseBonus > 0);
            
            RefreshUI();
        }
    }

    private void InitializeAnimation()
    {
        if (playerAnimation == null)
        {
            playerAnimation = GetComponent<PlayerAnimation>();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, amount), 0, maxHealth);
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

        // 현재 공격력 계산
        int currentAttack = AttackValue;

        // 공격 애니메이션 재생
        if (playerAnimation != null)
        {
            playerAnimation.PlayAttackAnimation(currentAttack);
        }

        // 공격 실행
        if (targets != null)
        {
            foreach (IDamageable target in targets)
            {
                if (target != null && target.IsAlive)
                {
                    target.TakeDamage(currentAttack);
                }

            }
        }

        // 공격 애니메이션이 끝날 때까지 대기
        if (playerAnimation != null)
        {
            yield return StartCoroutine(playerAnimation.WaitForAttackAnimationComplete(currentAttack));
        }

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

    private void CacheSpriteOrigin()
    {
        if (spriteTransform != null && !hasCachedSpriteOrigin)
        {
            initialSpriteLocalPosition = spriteTransform.localPosition;
            hasCachedSpriteOrigin = true;
        }
    }

    public override void TakeDamage(int amount)
    {
        float actualDamage = Mathf.Max(0, amount - cardDefenseBonus);
        currentHealth = Mathf.Clamp(currentHealth - Mathf.RoundToInt(actualDamage), 0, maxHealth);
        RefreshUI();
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        if (attackText != null)
        {
            attackText.text = AttackValue.ToString();
        }
    }
}

