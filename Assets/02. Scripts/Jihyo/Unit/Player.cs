using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    
    [Header("UI Animation")]
    [SerializeField] private float statAnimationDuration = 0.5f;
    [SerializeField] private Ease statAnimationEase = Ease.OutQuad;
    
    private Tweener attackTextTweener;
    private Tweener protectionTweener;

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

    /// 공격 시 카드 스탯을 업데이트합니다.
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
            playerAnimation.TriggerAttackByValue(currentAttack);
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
    
    public IEnumerator MoveToAttackPosition(Vector3? attackAnchorPosition, bool isAreaAttack)
    {
        CacheSpriteOrigin();
        
        if (spriteTransform == null)
        {
            yield break;
        }
        
        Vector3 attackPosition = initialSpriteLocalPosition;
        
        if (isAreaAttack)
        {
            attackPosition.x = 0f;
        }
        else if (attackAnchorPosition.HasValue)
        {
            Transform parent = spriteTransform.parent;
            Vector3 targetLocal = parent != null
                ? parent.InverseTransformPoint(attackAnchorPosition.Value)
                : attackAnchorPosition.Value;
            
            attackPosition.x = targetLocal.x - Mathf.Abs(singleTargetAttackOffset);
        }
        else
        {
            attackPosition.x = initialSpriteLocalPosition.x - Mathf.Abs(singleTargetAttackOffset);
        }
        
        yield return StartCoroutine(MoveSpriteToPosition(attackPosition, attackMoveDuration));
    }
    
    public IEnumerator ReturnToOriginalPosition()
    {
        if (spriteTransform != null)
        {
            yield return StartCoroutine(MoveSpriteToPosition(initialSpriteLocalPosition, returnMoveDuration));
        }
    }

    public override void TakeDamage(int amount)
    {
        // 데미지 받기 전에 카드 스탯 업데이트
        UpdateCardStats();
        
        // 보호력 먼저 감소, 남은 데미지는 체력으로
        base.TakeDamage(amount);
    }
    public void ApplyFieldStatsToPlayer()
    {
        if (GameData.Instance != null)
        {
            // 기본 공격력에 카드 필드의 공격력을 더함
            float fieldAttackPower = GameData.Instance.AttackField();
            int currentAttack = AttackValue;
            int targetAttack = Mathf.RoundToInt(baseAttack + fieldAttackPower);
            
            cardAttackBonus = fieldAttackPower;
            lastAttackBonus = cardAttackBonus;
            
            // 기존 보호력이 있으면 그 값에 더하고, 없으면 새로 설정
            float fieldDefensePower = GameData.Instance.DefenseField();
            float currentProtection = protectionValue;
            float targetProtection = protectionValue > 0 ? protectionValue + fieldDefensePower : fieldDefensePower;
            
            cardDefenseBonus = fieldDefensePower;
            lastDefenseBonus = cardDefenseBonus;
            
            // 공격력과 보호력 애니메이션
            AnimateAttackText(currentAttack, targetAttack);
            AnimateProtection(currentProtection, targetProtection);
        }
    }
    
    private void AnimateAttackText(int fromValue, int toValue)
    {
        if (attackText == null)
        {
            return;
        }
        
        if (attackTextTweener != null && attackTextTweener.IsActive())
        {
            attackTextTweener.Kill();
        }
        
        int currentValue = fromValue;
        attackTextTweener = DOTween.To(
            () => currentValue,
            x => {
                currentValue = x;
                attackText.text = currentValue.ToString();
            },
            toValue,
            statAnimationDuration
        ).SetEase(statAnimationEase);
    }
    
    private void AnimateProtection(float fromValue, float toValue)
    {
        if (Mathf.Abs(fromValue - toValue) < 0.01f)
        {
            RefreshUI();
            return;
        }
        
        if (protectionTweener != null && protectionTweener.IsActive())
        {
            protectionTweener.Kill();
        }
        
        float currentValue = fromValue;
        protectionTweener = DOTween.To(
            () => currentValue,
            x => {
                currentValue = x;
                protectionValue = currentValue;
                hasDefense = currentValue > 0;
                RefreshUI();
            },
            toValue,
            statAnimationDuration
        ).SetEase(statAnimationEase);
    }
    
    public void ResetAttackToBase()
    {
        cardAttackBonus = 0;
        lastAttackBonus = 0;
        
        // TODO: 추후 보호력이 남는 효과 추가 시 수정
        SetProtection(0);
        cardDefenseBonus = 0;
        lastDefenseBonus = 0;
        
        RefreshUI();
    }
    
    protected override void RefreshUI()
    {
        base.RefreshUI();

        if (attackText != null && (attackTextTweener == null || !attackTextTweener.IsActive()))
        {
            attackText.text = AttackValue.ToString();
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (attackTextTweener != null && attackTextTweener.IsActive())
        {
            attackTextTweener.Kill();
        }
        
        if (protectionTweener != null && protectionTweener.IsActive())
        {
            protectionTweener.Kill();
        }
    }
}

