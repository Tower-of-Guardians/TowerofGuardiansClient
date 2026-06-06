using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jongmin
{
    public class HandDomain : MonoBehaviour, IDropHandler
    {
        [BigHeader("Inner References")]
        [SerializeField] private HandUIDesigner handDesigner;
        [SerializeField] private HandView handView;
        [SerializeField] private HandSystem handSystem;
        [SerializeField] private HandEventSystem handEventSystem;

        [Space(30f), BigHeader("Outer References")]
        [SerializeField] private CardInfoUI cardInfoUI;
        [SerializeField] private TurnManager turnManager;

        private CardContainer _cardContainer;
        private HandCardLayout _cardLayout;
        private HandCardFactory _cardFactory;

        public void Construct(CardDropSystem dropSystem, CardContainer cardContainer)
        {
            _cardContainer = cardContainer ?? new CardContainer();
            _cardLayout = new HandCardLayout(handSystem, handDesigner, _cardContainer);
            _cardFactory = new HandCardFactory(handView, handEventSystem);

            handSystem.Construct(_cardContainer, _cardLayout, _cardFactory);
            handEventSystem.Construct(handSystem, dropSystem, _cardContainer);
        }

        public void BindEvents()
        {
            handEventSystem.OnPointerEntered += HandleOnPointerEnter;
            handEventSystem.OnPointerExited += HandleOnPointerExit;
            handEventSystem.OnPointerClicked += HandleOnPointerClick;
            handEventSystem.OnDragCanceled += HandleOnDragCanceled;
            handEventSystem.RequestBeginDrag += HandleRequestBeginDrag;
            handEventSystem.RequestSwapInSameField += HandleRequestSwapInSameField;
            handEventSystem.RequestChangeDropState += HandleRequestChangeDropState;
            handEventSystem.RequestEndDrag += HandleRequestEndDrag;
        }

        public void ReleaseEvents()
        {
            handEventSystem.OnPointerEntered -= HandleOnPointerEnter;
            handEventSystem.OnPointerExited -= HandleOnPointerExit;
            handEventSystem.OnPointerClicked -= HandleOnPointerClick;
            handEventSystem.OnDragCanceled -= HandleOnDragCanceled;
            handEventSystem.RequestBeginDrag -= HandleRequestBeginDrag;
            handEventSystem.RequestSwapInSameField -= HandleRequestSwapInSameField;
            handEventSystem.RequestChangeDropState -= HandleRequestChangeDropState;
            handEventSystem.RequestEndDrag -= HandleRequestEndDrag;
        }

        public void OnDrop(PointerEventData eventData)
        {
            handEventSystem?.OnDrop(eventData);
        }

        private void HandleOnPointerEnter(Card card)
        {
            handSystem.HoverCard = card;
            _cardLayout.UpdateLayout();
        }

        private void HandleOnPointerExit()
        {
            handSystem.HoverCard = null;
            _cardLayout.UpdateLayout();
        }

        private void HandleOnPointerClick(CardData cardData)
        {
            cardInfoUI?.ShowCardInfo(cardData);
        }

        private void HandleOnDragCanceled()
        {
            handSystem.ToggleFieldPreview(false);
            handView.TogglePreview(false);
            _cardLayout.UpdateLayout();
        }

        private void HandleRequestBeginDrag()
        {
            if (!turnManager.CanAction)
            {
                return;
            }

            UpdatePreviewCard();
        }

        private void HandleRequestSwapInSameField(Card card, Vector2 position)
        {
            InsertInSameField(card, position);
            UpdatePreviewCard();
        }

        private void HandleRequestChangeDropState(bool canDrop)
        {
            handSystem.HoverCard?.DOKill();
            handSystem.HoverCard?.transform.DOScale(canDrop ? 0.8f : handDesigner.Scale, handDesigner.AnimeSPD);
        }

        private void HandleRequestEndDrag()
        {
            handSystem.ToggleFieldPreview(false);
            handSystem.HoverCard = null;

            handView.TogglePreview(false);
            _cardLayout.UpdateLayout();
            SyncWithGameData();
        }

        private void UpdatePreviewCard()
        {
            if (handSystem.HoverCard == null || !_cardContainer.IsExist(handSystem.HoverCard))
            {
                handView.TogglePreview(false);
                return;
            }

            if (!_cardContainer.TryGetIndex(handSystem.HoverCard, out var index))
            {
                return;
            }

            var layoutData = CardLayoutCalculator.CalculatedHandCardTransform(
                index,
                _cardContainer.Count,
                handDesigner.Radius,
                handDesigner.Angle,
                handDesigner.Depth);

            handView.TogglePreview(true);
            handView.UpdatePreviewPosition(layoutData);
        }

        private void InsertInSameField(Card card, Vector2 position)
        {
            if (_cardContainer.IsPriority(handSystem.HoverCard, card))
            {
                if (position.x > card.transform.position.x)
                {
                    _cardContainer.Insert(handSystem.HoverCard, card);
                    _cardLayout.UpdateLayout(true);
                }
            }
            else
            {
                if (position.x < card.transform.position.x)
                {
                    _cardContainer.Insert(handSystem.HoverCard, card);
                    _cardLayout.UpdateLayout(true);
                }
            }
        }

        private void SyncWithGameData()
        {
            for (var i = 0; i < _cardContainer.Count; i++)
            {
                var cardId = _cardContainer.Get(i).BattleCardData.data.id;

                if (i < GameData.Instance.handDeck.Count)
                {
                    GameData.Instance.handDeck[i] = cardId;
                }
                else
                {
                    GameData.Instance.handDeck.Add(cardId);
                }
            }

            while (GameData.Instance.handDeck.Count > _cardContainer.Count)
            {
                GameData.Instance.handDeck.RemoveAt(GameData.Instance.handDeck.Count - 1);
            }
        }

        private void OnDestroy()
        {
            ReleaseEvents();
        }
    }
}
