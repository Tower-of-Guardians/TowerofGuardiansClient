using DG.Tweening;
using UnityEngine;

namespace Jongmin
{
    public class FieldDomain : MonoBehaviour
    {
        [SerializeField] private FieldView atkView;
        [SerializeField] private FieldView defView;
        [SerializeField] private FieldSystem atkFieldSystem;
        [SerializeField] private FieldSystem defFieldSystem;
        [SerializeField] private FieldEventSystem atkFieldEventSystem;
        [SerializeField] private FieldEventSystem defFieldEventSystem;
        [SerializeField] private PreviewCard atkPreviewCard;
        [SerializeField] private PreviewCard defPreviewCard;
        [SerializeField] private FieldUIDesigner designer;
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private DiscardSystem discardSystem;

        private CardContainer _atkCardContainer;
        private CardContainer _defCardContainer;
        private FieldCardLayout _atkCardLayout;
        private FieldCardLayout _defCardLayout;
        private FieldCardFactory _atkCardFactory;
        private FieldCardFactory _defCardFactory;
        
        public AtkFieldSystem AtkSystem => atkFieldSystem as AtkFieldSystem;
        public DefFieldSystem DefSystem => defFieldSystem as DefFieldSystem;
        
        public void Construct(CardDropSystem cardDropSystem)
        {
            _atkCardContainer = new CardContainer();
            _defCardContainer = new CardContainer();
            _atkCardLayout = new FieldCardLayout(atkFieldSystem, designer, _atkCardContainer, atkPreviewCard);
            _defCardLayout = new FieldCardLayout(defFieldSystem, designer, _defCardContainer, defPreviewCard);
            _atkCardFactory = new FieldCardFactory(atkView, atkFieldEventSystem);
            _defCardFactory = new FieldCardFactory(defView, defFieldEventSystem);
            
            atkFieldSystem.Construct(atkView, _atkCardContainer, _atkCardLayout, _atkCardFactory);
            defFieldSystem.Construct(defView, _defCardContainer, _defCardLayout, _defCardFactory);
            
            atkFieldEventSystem.Construct(atkFieldSystem, cardDropSystem, _atkCardContainer);
            defFieldEventSystem.Construct(defFieldSystem, cardDropSystem, _defCardContainer);

            BindEvents();
        }

        public void BindEvents()
        {
            atkFieldSystem.RequestUpdateActionCount += turnManager.UpdateActionCount;
            defFieldSystem.RequestUpdateActionCount += turnManager.UpdateActionCount;

            discardSystem.OnDiscardViewVisibilityChanged += atkFieldSystem.UpdateInteraction;
            discardSystem.OnDiscardViewVisibilityChanged += defFieldSystem.UpdateInteraction;

            atkFieldEventSystem.RequestOnBeginDrag += HandleRequestOnBeginDrag;
            atkFieldEventSystem.RequestSwapInSameField += HandleRequestSwapInSameField;
            atkFieldEventSystem.RequestOnEndDrag += HandleRequestOnEndDrag;
            atkFieldEventSystem.RequestMoveHoverCardToOpposite += HandleMoveHoverCardToOpposite;
            
            defFieldEventSystem.RequestOnBeginDrag += HandleRequestOnBeginDrag;
            defFieldEventSystem.RequestSwapInSameField += HandleRequestSwapInSameField;
            defFieldEventSystem.RequestOnEndDrag += HandleRequestOnEndDrag;
            defFieldEventSystem.RequestMoveHoverCardToOpposite += HandleMoveHoverCardToOpposite;
        }

        public void ReleaseEvents()
        {
            atkFieldSystem.RequestUpdateActionCount -= turnManager.UpdateActionCount;
            defFieldSystem.RequestUpdateActionCount -= turnManager.UpdateActionCount;
            
            discardSystem.OnDiscardViewVisibilityChanged -= atkFieldSystem.UpdateInteraction;
            discardSystem.OnDiscardViewVisibilityChanged -= defFieldSystem.UpdateInteraction;
            
            atkFieldEventSystem.RequestOnBeginDrag -= HandleRequestOnBeginDrag;
            atkFieldEventSystem.RequestSwapInSameField -= HandleRequestSwapInSameField;
            atkFieldEventSystem.RequestOnEndDrag -= HandleRequestOnEndDrag;
            atkFieldEventSystem.RequestMoveHoverCardToOpposite -= HandleMoveHoverCardToOpposite;
            
            defFieldEventSystem.RequestOnBeginDrag -= HandleRequestOnBeginDrag;
            defFieldEventSystem.RequestSwapInSameField -= HandleRequestSwapInSameField;
            defFieldEventSystem.RequestOnEndDrag -= HandleRequestOnEndDrag;
            defFieldEventSystem.RequestMoveHoverCardToOpposite -= HandleMoveHoverCardToOpposite;
        }

        private void HandleRequestOnBeginDrag(Card card, FieldType fieldType)
        {
            if (fieldType == FieldType.Attack)
            {
                atkFieldSystem.HoverCard = card;
                UpdatePreviewPosition(FieldType.Attack);
            }
            else
            {
                defFieldSystem.HoverCard = card;
                UpdatePreviewPosition(FieldType.Defense);
            }

            MoveHoverCardToCanvas(card);
        }

        private void HandleRequestSwapInSameField(Card card, FieldType fieldType, Vector2 position)
        {
            var system = fieldType == FieldType.Attack ? atkFieldSystem : defFieldSystem;
            var container = fieldType == FieldType.Attack ? _atkCardContainer : _defCardContainer;
            var layout = fieldType == FieldType.Attack ? _atkCardLayout : _defCardLayout;
            
            InsertInSameField(card, system, container, layout, position);
            UpdatePreviewPosition(fieldType);
        }

        private void HandleRequestOnEndDrag(FieldType fieldType)
        {
            var currentView = fieldType == FieldType.Attack ? atkView : defView;
            var oppositeView = fieldType == FieldType.Attack ? defView : atkView;
            
            var system = fieldType == FieldType.Attack ? atkFieldSystem : defFieldSystem;
            var eventSystem = fieldType == FieldType.Attack ? atkFieldEventSystem : defFieldEventSystem;

            var fieldFlag = eventSystem.TryInsertInOppositeFieldWithField();
            var cardFlag = eventSystem.TryInsertInOppositeFieldWithCard();

            var hoverCardWorldPosition = system.HoverCard.transform.position;
            var finalHoverCardRoot = fieldFlag || cardFlag ? oppositeView.CardRoot : currentView.CardRoot;
            
            system.HoverCard.transform.SetParent(finalHoverCardRoot, false);
            
            var hoverCardLocalPosition = system.HoverCard.transform.parent.InverseTransformPoint(hoverCardWorldPosition);
            system.HoverCard.transform.localPosition = hoverCardLocalPosition;
        }
        
        private void HandleMoveHoverCardToOpposite(FieldType fieldType)
        {
            var currentField = fieldType == FieldType.Attack ? atkFieldSystem : defFieldSystem;
            var hoverCard = currentField.HoverCard;

            var currentContainer = fieldType == FieldType.Attack ? _atkCardContainer : _defCardContainer;
            var oppositeContainer = fieldType == FieldType.Attack ? _defCardContainer : _atkCardContainer;
            
            var currentEventSystem = fieldType == FieldType.Attack ? atkFieldEventSystem : defFieldEventSystem;
            var oppositeEventSystem = fieldType == FieldType.Attack ? defFieldEventSystem : atkFieldEventSystem;
            
            currentEventSystem.Unsubscribe(hoverCard);
            oppositeEventSystem.Subscribe(hoverCard);
            
            hoverCard.View.ToggleLock();
            
            oppositeContainer.Add(hoverCard);
            currentContainer.Remove(hoverCard);

            SyncAtkDataWithContainer();
            SyncDefDataWithContainer();
        }

        private void InsertInSameField(Card card, FieldSystem system, CardContainer container, FieldCardLayout layout, Vector2 position)
        {
            if (container.IsPriority(system.HoverCard, card))
            {
                if (position.x > card.transform.position.x)
                {
                    container.Insert(system.HoverCard, card);
                    layout.UpdateLayout(false);
                }
            }
            else
            {
                if (position.x < card.transform.position.x)
                {
                    container.Insert(system.HoverCard, card);
                    layout.UpdateLayout(false);
                }
            }
        }

        private void MoveHoverCardToCanvas(Card card)
        {
            card.DOKill();
            card.transform.SetParent(rootCanvas.transform, false);
        }

        private void UpdatePreviewPosition(FieldType fieldType)
        {
            if (fieldType == FieldType.Attack)
            {
                if (!_atkCardContainer.TryGetIndex(atkFieldSystem.HoverCard, out var atkIndex))
                {
                    return;
                }
                
                var previewPosition = CardLayoutCalculator.CalculatedFieldCardPosition(atkIndex, designer.ATKLimit, designer.Space);
                atkView.UpdatePreviewPosition(previewPosition);
            }
            else
            {
                if (!_defCardContainer.TryGetIndex(defFieldSystem.HoverCard, out var defIndex))
                {
                    return;
                }
                
                var previewPosition = CardLayoutCalculator.CalculatedFieldCardPosition(defIndex, designer.ATKLimit, designer.Space);
                defView.UpdatePreviewPosition(previewPosition);
            }
        }

        private void SyncAtkDataWithContainer()
        {
            var cards = _atkCardContainer.Cards;

            for (var i = 0; i < cards.Count; i++)
            {
                if (i < GameData.Instance.attackField.Count)
                {
                    GameData.Instance.attackField[i] = cards[i].CardData;
                }
                else
                {
                    GameData.Instance.attackField.Add(cards[i].CardData);
                }
            }

            while (GameData.Instance.attackField.Count > cards.Count)
            {
                GameData.Instance.attackField.RemoveAt(cards.Count - 1);
            }
        }

        private void SyncDefDataWithContainer()
        {
            var cards = _defCardContainer.Cards;

            for (var i = 0; i < cards.Count; i++)
            {
                if (i < GameData.Instance.defenseField.Count)
                {
                    GameData.Instance.defenseField[i] = cards[i].CardData;
                }
                else
                {
                    GameData.Instance.defenseField.Add(cards[i].CardData);
                }
            }

            while (GameData.Instance.defenseField.Count > cards.Count)
            {
                GameData.Instance.defenseField.RemoveAt(cards.Count - 1);
            }
        }

        private void OnDestroy()
        {
            ReleaseEvents();
        }
    }
}