using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Monster : BaseUnit, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] private const int Attack = 5;

    [Header("Status UI")]
    [SerializeField] private Transform attackAnchor;
    [SerializeField] private GameObject targetIndicator;

    [Header("Animation")]
    private MonsterAnimation monsterAnimation;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [SerializeField] private float attackMoveOffset = 0f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    
    [Header("Attack Timing")]
    [SerializeField] private float damageApplyDelay = 0.5f;

    private Vector3 initialSpriteLocalPosition;
    private bool hasSavedInitialPosition;
    private const int NormalSortingOrder = 5;
    private const int AttackSortingOrder = 7;

    public event Action<Monster> Clicked;
    private BattleManager battleManager;
    private Coroutine registrationRoutine;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
        InitializeAnimation();
        SaveInitialPosition();
        SetTargeted(false);
        RegisterBattleManager();
    }

    private void InitializeAnimation()
    {
        if (monsterAnimation == null)
        {
            monsterAnimation = GetComponent<MonsterAnimation>();
        }
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

    public IEnumerator PerformAttack(IDamageable target)
    {
        if (target == null || !target.IsAlive)
        {
            yield break;
        }

        SaveInitialPosition();

        SetSortingOrder(AttackSortingOrder);

        if (attackMoveOffset > 0f)
        {
            Vector3 targetPosition = GetTargetAttackPosition(target);
            yield return StartCoroutine(MoveSpriteToPosition(targetPosition, attackMoveDuration));
        }

        if (monsterAnimation != null)
        {
            monsterAnimation.PlayAttackAnimation();
        }

        // 공격 중 체력이 감소하는 타이밍
        yield return new WaitForSeconds(damageApplyDelay);

        int damage = GetAttackValue();
        if (damage > 0)
        {
            target.TakeDamage(damage);
        }

        if (monsterAnimation != null)
        {
            yield return StartCoroutine(monsterAnimation.WaitForAttackAnimationComplete());
        }

        if (attackMoveOffset > 0f)
        {
            yield return StartCoroutine(MoveSpriteToPosition(initialSpriteLocalPosition, attackMoveDuration));
        }

        SetSortingOrder(NormalSortingOrder);
    }

    private Vector3 GetTargetAttackPosition(IDamageable target)
    {
        if (target == null)
        {
            return initialSpriteLocalPosition;
        }

        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono == null)
        {
            return initialSpriteLocalPosition;
        }

        Vector3 targetWorldPosition = targetMono.transform.position;
        
        Transform parent = transform.parent;
        Vector3 targetLocal = parent != null
            ? parent.InverseTransformPoint(targetWorldPosition)
            : targetWorldPosition;

        Vector3 direction = (targetLocal - initialSpriteLocalPosition).normalized;
        Vector3 attackPosition = targetLocal - direction * attackMoveOffset;
        
        attackPosition.y = initialSpriteLocalPosition.y;

        return attackPosition;
    }

    private void SaveInitialPosition()
    {
        if (!hasSavedInitialPosition)
        {
            initialSpriteLocalPosition = transform.localPosition;
            hasSavedInitialPosition = true;
        }
    }

    private IEnumerator MoveSpriteToPosition(Vector3 targetPosition, float duration)
    {
        targetPosition.y = initialSpriteLocalPosition.y;
        
        if (duration <= 0f)
        {
            Vector3 finalPosition = transform.localPosition;
            finalPosition.x = targetPosition.x;
            finalPosition.y = initialSpriteLocalPosition.y;
            transform.localPosition = finalPosition;
            yield break;
        }

        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y = initialSpriteLocalPosition.y;
            transform.localPosition = currentPosition;
            yield return null;
        }

        Vector3 finalPos = targetPosition;
        finalPos.y = initialSpriteLocalPosition.y;
        transform.localPosition = finalPos;
    }

    private void SetSortingOrder(int sortingOrder)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
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

    private bool isMarkedForDeath = false;

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        
        if (monsterAnimation != null && IsAlive)
        {
            monsterAnimation.PlayHitAnimation();
        }
        
        if (!IsAlive && !isMarkedForDeath)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isMarkedForDeath = true;
    }

    public void DestroyMonster()
    {
        if (battleManager != null)
        {
            battleManager.UnregisterMonster(this);
        }
        
        Destroy(gameObject);
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        if (attackText != null)
        {
            attackText.text = Attack.ToString();
        }
    }
}
