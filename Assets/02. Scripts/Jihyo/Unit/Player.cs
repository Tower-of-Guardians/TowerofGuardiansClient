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
    private float lastAttackBonus;
    private float lastDefenseBonus;

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
            currentHealth = maxHealth;
            
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
            float newDefenseBonus = GameData.Instance.DefenseField();
            
            if (newDefenseBonus > cardDefenseBonus)
            {
                float protectionIncrease = newDefenseBonus - cardDefenseBonus;
                AddProtection(protectionIncrease);
            }
            else if (newDefenseBonus < cardDefenseBonus)
            {
                float protectionDecrease = cardDefenseBonus - newDefenseBonus;
                if (protectionValue > 0)
                {
                    protectionValue = Mathf.Max(0, protectionValue - protectionDecrease);
                    if (protectionValue <= 0)
                    {
                        hasDefense = false;
                    }
                }
            }
            
            cardDefenseBonus = newDefenseBonus;
            lastAttackBonus = cardAttackBonus;
            lastDefenseBonus = cardDefenseBonus;
            
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
        // 공격 전 카드 스탯 업데이트
        UpdateCardStats();
        
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

        // TODO: 공격 애니메이션 후 0.5초 대기 후 데미지 적용(하드코딩)
        yield return new WaitForSeconds(0.5f);

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
        // 데미지 받기 전에 카드 스탯 업데이트
        UpdateCardStats();
        
        // 보호력 먼저 감소, 남은 데미지는 체력으로
        base.TakeDamage(amount);
    }

    private void Update()
    {
        // 카드를 필드에 올릴 때마다 공격력과 보호력 UI 갱신
        if (GameData.Instance != null)
        {
            float currentAttackBonus = GameData.Instance.AttackField();
            float currentDefenseBonus = GameData.Instance.DefenseField();
            
            // 값이 변경되었을 때만 업데이트
            if (Mathf.Abs(currentAttackBonus - lastAttackBonus) > 0.01f || 
                Mathf.Abs(currentDefenseBonus - lastDefenseBonus) > 0.01f)
            {
                UpdateCardStats();
                lastAttackBonus = currentAttackBonus;
                lastDefenseBonus = currentDefenseBonus;
            }
        }
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

