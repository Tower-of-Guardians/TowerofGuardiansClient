using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private bool playerAttackHitsAll;
    [SerializeField] private float monsterAttackDelay = 0.15f;

    private readonly List<Monster> primaryMonsters = new();
    private readonly List<Monster> monstersToRemove = new();
    private Player player;
    private Button attackButton;
    private Monster selectedTarget;
    private bool isProcessingAttack;
    private bool isInitialized;
    private int throwCountBeforeAction;
    private int lastThrowCountBeforeRemoveAll;

    private void OnDestroy()
    {
        DetachAttackButton();
        DetachThrowPresenter();
        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.Clicked -= OnMonsterClicked;
            }
        }
        primaryMonsters.Clear();
        selectedTarget = null;
    }

    private void OnMonsterClicked(Monster monster)
    {
        if (monster == null || !monster.IsAlive)
        {
            return;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
            return;
        }

        if (selectedTarget != null)
        {
            selectedTarget.SetTargeted(false);
        }

        selectedTarget = monster;
        selectedTarget.SetTargeted(true);
    }

    public void Initialize(Player playerUnit, IEnumerable<Monster> monsters, Button attackBtn)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleManager has already been initialized.");
            return;
        }

        player = playerUnit;
        attackButton = attackBtn;
        AttachAttackButton();

        if (monsters != null)
        {
            foreach (Monster monster in monsters)
            {
                RegisterMonster(monster);
            }
        }

        isInitialized = true;

        // ThrowPresenter 이벤트 구독
        AttachThrowPresenter();

        // 처음 카드 뽑기
        StartCoroutine(DrawCardsAtTurnStartDelayed());
    }

    public void RegisterMonster(Monster monster)
    {
        if (monster == null || primaryMonsters.Contains(monster))
        {
            return;
        }

        primaryMonsters.Add(monster);
        monster.Clicked += OnMonsterClicked;
    }

    public void UnregisterMonster(Monster monster)
    {
        if (monster == null)
        {
            return;
        }

        if (primaryMonsters.Remove(monster))
        {
            monster.Clicked -= OnMonsterClicked;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
        }

        monstersToRemove.Remove(monster);
    }

    public void MarkMonsterForRemove(Monster monster)
    {
        if (monster != null && !monstersToRemove.Contains(monster))
        {
            monstersToRemove.Add(monster);
        }
    }

    private void RemoveDeadMonsters()
    {
        foreach (Monster monster in monstersToRemove.ToList())
        {
            if (monster != null)
            {
                monster.DestroyMonster();
            }
        }
        monstersToRemove.Clear();
    }

    public void ConfigureAttackButton(Button button)
    {
        if (attackButton == button)
        {
            return;
        }

        DetachAttackButton();
        attackButton = button;
        AttachAttackButton();
    }

    public void SetPlayer(Player playerUnit)
    {
        player = playerUnit;
    }

    private void HandleAttackButton()
    {
        if (isProcessingAttack || player == null)
        {
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isProcessingAttack = true;

        // 매 턴마다 Throw 시스템 초기화
        if (DIContainer.IsRegistered<TurnManager>())
        {
            var turnManager = DIContainer.Resolve<TurnManager>();
            turnManager.Initialize();

            throwCountBeforeAction = 0;
        }

        List<Monster> aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("No monsters available to attack.");
            isProcessingAttack = false;
            yield break;
        }

        List<IDamageable> playerTargets = new();
        Monster primaryMonsterTarget = null;
        if (playerAttackHitsAll)
        {
            playerTargets.AddRange(aliveMonsters);
            if (aliveMonsters.Count > 0)
            {
                primaryMonsterTarget = aliveMonsters[0];
            }
        }
        else
        {
            Monster target = selectedTarget != null && selectedTarget.IsAlive
                ? selectedTarget
                : aliveMonsters[Random.Range(0, aliveMonsters.Count)];

            primaryMonsterTarget = target;
            playerTargets.Add(target);
        }

        Vector3? attackAnchorPosition = primaryMonsterTarget != null
            ? primaryMonsterTarget.AttackAnchor.position
            : null;

        yield return StartCoroutine(player.PerformAttack(playerTargets, playerAttackHitsAll, attackAnchorPosition));

        // 공격 완료 후 죽은 몬스터들 제거
        RemoveDeadMonsters();

        // 모든 몬스터 처치 확인
        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            isProcessingAttack = false;
            yield return StartCoroutine(HandleVictory());
            yield break;
        }

        // 몬스터 공격 대기
        yield return new WaitForSeconds(0.5f);

        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }
        selectedTarget = null;

        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();

        foreach (Monster monster in aliveMonsters)
        {
            if (monster == null || !monster.IsAlive)
            {
                continue;
            }

            monster.PerformAttack(player);

            if (monsterAttackDelay > 0f)
            {
                yield return new WaitForSeconds(monsterAttackDelay);
            }

            if (!player.IsAlive)
            {
                Debug.Log("Player defeated.");
                isProcessingAttack = false;
                yield return StartCoroutine(HandleDefeat());
                yield break;
            }
        }

        aliveMonsters = primaryMonsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            isProcessingAttack = false;
            yield return StartCoroutine(HandleVictory());
            yield break;
        }

        // 몬스터 공격 후 죽은 몬스터들 제거
        RemoveDeadMonsters();

        isProcessingAttack = false;

        // 턴 종료 후 사용하지 않은 핸드 카드 모두 버리기
        DiscardAllHandCards();

        // 턴 종료 후 기본 드로우
        DrawCards();
    }

    private IEnumerator DrawCardsAtTurnStartDelayed()
    {
        // HandPresenter와 TurnManager가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<HandPresenter>() && DIContainer.IsRegistered<TurnManager>());
        
        // 덱이 준비될 때까지 대기
        yield return new WaitUntil(() => GameData.Instance != null && GameData.Instance.notuseDeck.Count > 0);

        DrawCards();
    }

    private void DiscardAllHandCards()
    {
        // 사용 카드 버리기
        if (GameData.Instance != null)
        {
            var attackFieldCards = new List<CardData>(GameData.Instance.attackField);
            foreach (var cardData in attackFieldCards)
            {
                if (cardData != null)
                {
                    GameData.Instance.UseCard(cardData.id);
                }
            }
            GameData.Instance.attackField.Clear();
            
            var defenseFieldCards = new List<CardData>(GameData.Instance.defenseField);
            foreach (var cardData in defenseFieldCards)
            {
                if (cardData != null)
                {
                    GameData.Instance.UseCard(cardData.id);
                }
            }
            GameData.Instance.defenseField.Clear();
        }
        
        // Hand UI 카드 제거
        if (DIContainer.IsRegistered<HandPresenter>())
        {
            var handPresenter = DIContainer.Resolve<HandPresenter>();
            
            // 핸드 카드 버리기
            var handCards = handPresenter.GetCardDatas();
            if (handCards != null && handCards.Length > 0)
            {
                foreach (var cardData in handCards)
                {
                    if (cardData != null && cardData.data != null)
                    {
                        // 버리는 덱에 추가
                        GameData.Instance.UseCard(cardData.data.id);
                    }
                }
            }

            handPresenter.ClearAllCards();
            GameData.Instance.handDeck.Clear();
        }
        
        // Attacking Field UI 카드 제거
        if (DIContainer.IsRegistered<AttackFieldPresenter>())
        {
            var attackFieldPresenter = DIContainer.Resolve<AttackFieldPresenter>();
            attackFieldPresenter.RemoveAll();
        }
        
        // Defending Field UI 카드 제거
        if (DIContainer.IsRegistered<DefendFieldPresenter>())
        {
            var defendFieldPresenter = DIContainer.Resolve<DefendFieldPresenter>();
            defendFieldPresenter.RemoveAll();
        }
    }

    private void DrawCards(int count = -1)
    {
        if (!DIContainer.IsRegistered<HandPresenter>() || !DIContainer.IsRegistered<TurnManager>())
        {
            return;
        }

        var handPresenter = DIContainer.Resolve<HandPresenter>();
        var turnManager = DIContainer.Resolve<TurnManager>();

        // count가 -1이면 기본값(MaxHandCount) 사용, 그 외에는 지정된 개수만큼 드로우
        int drawCount = count >= 0 ? count : turnManager.MaxHandCount;
        
        for (int i = 0; i < drawCount; i++)
        {
            var cardData = GameData.Instance.NextDeckSet(1);
            if (cardData == null || cardData.data == null)
            {
                Debug.LogWarning($"BattleManager: 카드를 뽑을 수 없습니다. (뽑은 횟수: {i}/{drawCount})");
                break;
            }
            handPresenter.InstantiateCard(cardData);
        }
    }

    private void AttachThrowPresenter()
    {
        // TurnManager가 등록될 때까지 대기 후 이벤트 구독
        StartCoroutine(AttachTurnManagerDelayed());
    }

    private IEnumerator AttachTurnManagerDelayed()
    {
        yield return new WaitUntil(() => DIContainer.IsRegistered<TurnManager>());
        
        var turnManager = DIContainer.Resolve<TurnManager>();
        turnManager.OnUpdatedThrowActionState += OnThrowActionStateChanged;
        turnManager.OnUpdatedThrowCount += OnThrowCountChanged;
    }

    private void DetachThrowPresenter()
    {
        if (DIContainer.IsRegistered<TurnManager>())
        {
            var turnManager = DIContainer.Resolve<TurnManager>();
            turnManager.OnUpdatedThrowActionState -= OnThrowActionStateChanged;
            turnManager.OnUpdatedThrowCount -= OnThrowCountChanged;
        }
    }

    private void OnThrowCountChanged(ActionData actionData)
    {
        // Throw 카운트가 증가할 때 저장
        if (actionData.Current > throwCountBeforeAction)
        {
            throwCountBeforeAction = actionData.Current;
            lastThrowCountBeforeRemoveAll = actionData.Current;
        }
    }

    private void OnThrowActionStateChanged(bool canThrow)
    {
        if (!canThrow)
        {
            int throwCount = throwCountBeforeAction;
            
            if (throwCount == 0 && lastThrowCountBeforeRemoveAll > 0)
            {
                throwCount = lastThrowCountBeforeRemoveAll;
            }
            
            if (throwCount > 0)
            {
                // 애니메이션 완료를 위해 약간의 지연 후 드로우
                StartCoroutine(DrawCardsAfterThrowDelayed(throwCount));
            }
            else
            {
            }
            
            lastThrowCountBeforeRemoveAll = 0;
            
            throwCountBeforeAction = 0;
        }
    }
    
    private IEnumerator DrawCardsAfterThrowDelayed(int throwCount)
    {
        float waitTime = throwCount * 0.3f;
        yield return new WaitForSeconds(waitTime);
        
        if (throwCount > 0)
        {
            // Throw한 카드 수만큼 드로우
            DrawCards(throwCount);
        }
    }

    private void AttachAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(HandleAttackButton);
        }
    }

    private void DetachAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveListener(HandleAttackButton);
        }
    }

    private IEnumerator HandleVictory()
    {
        // 보상 계산 (TODO: 몬스터 데이터에서 보상 정보 가져오기)
        int totalGold = CalculateTotalGold();
        int totalExp = CalculateTotalExp();

        // ResultPresenter가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<ResultPresenter>());

        // Result 창 열기
        var resultPresenter = DIContainer.Resolve<ResultPresenter>();
        var resultData = new ResultData(BattleResultType.Victory, totalGold, totalExp);
        resultPresenter.OpenUI(resultData);
    }

    private IEnumerator HandleDefeat()
    {
        // ResultPresenter가 등록될 때까지 대기
        yield return new WaitUntil(() => DIContainer.IsRegistered<ResultPresenter>());

        // Result 창 열기
        var resultPresenter = DIContainer.Resolve<ResultPresenter>();
        var resultData = new ResultData(BattleResultType.Defeat, 0, 0);
        resultPresenter.OpenUI(resultData);
    }

    private int CalculateTotalGold()
    {
        // TODO: 몬스터 데이터에서 골드 정보 가져오기

        // 현재는 기본값 반환
        int totalGold = 0;
        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                // 몬스터당 기본 골드 (나중에 몬스터 데이터에서 가져오도록 수정)
                totalGold += 100;
            }
        }
        return totalGold;
    }

    private int CalculateTotalExp()
    {
        // TODO: 몬스터 데이터에서 경험치 정보 가져오기
        
        // 현재는 기본값 반환
        int totalExp = 0;
        foreach (Monster monster in primaryMonsters)
        {
            if (monster != null)
            {
                // 몬스터당 기본 경험치 (나중에 몬스터 데이터에서 가져오도록 수정)
                totalExp += 50;
            }
        }
        return totalExp;
    }
}