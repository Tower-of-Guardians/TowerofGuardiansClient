using System.Collections.Generic;
using UnityEngine;

public class BattleTurnEndController : MonoBehaviour, IBattleController
{
    private BattleManager battleManager;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    public void Initialize(BattleManager manager)
    {
        if (isInitialized)
        {
            Debug.LogWarning("BattleTurnEndController has already been initialized.");
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

    public void ProcessTurnEnd()
    {
        // 턴 종료 시 플레이어의 공격력을 기본 공격력으로 되돌림
        if (battleManager != null)
        {
            var setupController = battleManager.GetSetupController();
            if (setupController != null)
            {
                Player player = setupController.GetPlayer();
                if (player != null)
                {
                    player.ResetAttackToBase();
                }
            }
        }
        
        // 사용하지 않은 핸드 카드 모두 버리기
        DiscardAllHandCards();

        // 턴 종료 후 기본 드로우
        DrawCards();
    }

    public void DrawCards(int count = -1)
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
                Debug.LogWarning($"BattleTurnEndController: 카드를 뽑을 수 없습니다. (뽑은 횟수: {i}/{drawCount})");
                break;
            }
            handPresenter.InstantiateCard(cardData);
        }
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
}

