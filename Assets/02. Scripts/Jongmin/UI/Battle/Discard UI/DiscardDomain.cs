using DG.Tweening;
using JxModule;
using UnityEngine;

namespace Jongmin
{
    public class DiscardDomain : MonoBehaviour
    {
        [BigHeader("Inner References")]
        [SerializeField] private DiscardUIDesigner discardDesigner;
        [SerializeField] private DiscardView discardView;
        [SerializeField] private DiscardSystem discardSystem;
        [SerializeField] private DiscardEventSystem discardEventSystem;
        [SerializeField] private PreviewCard previewCard;
        
        [Space(30f), BigHeader("Outer References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private Canvas rootCanvas;
        
        private CardContainer _cardContainer;
        private DiscardCardLayout _cardLayout;
        private DiscardCardFactory _cardFactory;

        private void Awake()
        {
            Construct(null);
        }

        public void Construct(CardDropSystem cardDropSystem)
        {
            _cardContainer = new CardContainer();
            _cardLayout = new DiscardCardLayout(discardSystem, discardDesigner, _cardContainer, previewCard);
            _cardFactory = new DiscardCardFactory(discardView, discardEventSystem);
            
            discardSystem.Construct(discardView, _cardContainer, _cardLayout, _cardFactory);
            discardEventSystem.Construct(discardSystem, cardDropSystem, _cardContainer);

            BindEvents();
        }

        public void BindEvents()
        {
            discardView.Bind(discardSystem);
            
            discardSystem.RequestUpdateThrowAction += turnManager.UpdateThrowAction;
            discardSystem.RequestUpdateThrowCount += turnManager.UpdateThrowCount;
            turnManager.OnUpdatedThrowActionState += discardSystem.UpdateOpenButtonState;
            turnManager.OnUpdatedThrowCount += discardSystem.UpdateDiscardCount;

            discardEventSystem.RequestOnBeginDrag += HandleRequestOnBeginDrag;
            discardEventSystem.RequestSwapInSameField += HandleRequestSwapInSameField;
            discardEventSystem.RequestOnEndDrag += HandleRequestOnEndDrag;
        }

        public void ReleaseEvents()
        {
            discardSystem.RequestUpdateThrowAction -= turnManager.UpdateThrowAction;
            discardSystem.RequestUpdateThrowCount -= turnManager.UpdateThrowCount;
            turnManager.OnUpdatedThrowActionState -= discardSystem.UpdateOpenButtonState;
            turnManager.OnUpdatedThrowCount -= discardSystem.UpdateDiscardCount;
            
            discardEventSystem.RequestOnBeginDrag -= HandleRequestOnBeginDrag;
            discardEventSystem.RequestSwapInSameField -= HandleRequestSwapInSameField;
            discardEventSystem.RequestOnEndDrag -= HandleRequestOnEndDrag;
        }

        private void HandleRequestOnBeginDrag(Card card)
        {
            discardSystem.HoverCard = card;

            MoveHoverCardToRoot(card);
            UpdatePreviewPosition();
        }

        private void HandleRequestSwapInSameField(Card card, Vector2 position)
        {
            InsertInSameField(card, position);
            UpdatePreviewPosition();
        }

        private void HandleRequestOnEndDrag()
        {
            MoveHoverCardToParent();
            
            discardSystem.HoverCard = null;
            _cardLayout.UpdateLayout(false);
            discardView.TogglePreview(false);
        }

        private void MoveHoverCardToRoot(Card card)
        {
            card.DOKill();
            card.transform.SetParent(rootCanvas.transform, false);
        }

        private void MoveHoverCardToParent()
        {
            discardSystem.HoverCard.transform.SetParent(discardView.CardRoot, false);
        }
        
        private void InsertInSameField(Card card, Vector2 position)
        {
            if (_cardContainer.IsPriority(discardSystem.HoverCard, card))
            {
                if (position.x > card.transform.position.x)
                {
                    _cardContainer.Insert(discardSystem.HoverCard, card);
                    _cardLayout.UpdateLayout(true);
                }
            }
            else
            {
                if (position.x < card.transform.position.x)
                {
                    _cardContainer.Insert(discardSystem.HoverCard, card);
                    _cardLayout.UpdateLayout(true);
                }
            }
        }

        private void UpdatePreviewPosition()
        {
            if (!_cardContainer.TryGetIndex(discardSystem.HoverCard, out var index))
            {
                return;
            }
            
            var layoutData = CardLayoutCalculator.CalculatedThrowCardPosition(index, _cardContainer.Count, discardDesigner.Space);
            discardView.TogglePreview(true);
            discardView.UpdatePreviewPosition(layoutData);
        }

        private void OnDestroy()
        {
            ReleaseEvents();
        }
    }
}