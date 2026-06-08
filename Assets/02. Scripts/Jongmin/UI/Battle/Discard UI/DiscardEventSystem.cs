using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jongmin
{
    public class DiscardEventSystem : MonoBehaviour, IDropHandler
    {
        private DiscardSystem _discardSystem;
        private CardDropSystem _dropSystem;
        private CardContainer _container;
        
        public event Action<Card> RequestOnBeginDrag;
        public event Action<Card, Vector2> RequestSwapInSameField;
        public event Action RequestOnEndDrag;

        public void Construct(DiscardSystem discardSystem, CardDropSystem dropSystem, CardContainer container)
        {
            _discardSystem = discardSystem;
            _dropSystem = dropSystem;
            _container = container;
        }

        public void Subscribe(Card card)
        {
            card.Pointer.OnBeginDragged += HandleOnBeginDrag;
            card.Pointer.OnDragged += HandleOnDrag;
            card.Pointer.OnEndDragged += HandleOnEndDrag;
        }

        public void Unsubscribe(Card card)
        {
            card.Pointer.OnBeginDragged -= HandleOnBeginDrag;
            card.Pointer.OnDragged -= HandleOnDrag;
            card.Pointer.OnEndDragged -= HandleOnEndDrag;
        }

        private void HandleOnBeginDrag(Card card, PointerEventData eventData)
        {
            RequestOnBeginDrag?.Invoke(card);
        }

        private void HandleOnDrag(Card card, PointerEventData eventData)
        {
            if (_discardSystem.HoverCard == null)
            {
                return;
            }

            MoveHoverCardToMousePosition(eventData.position);
            RequestSwapInSameField?.Invoke(card, eventData.position);
        }

        private void HandleOnEndDrag(Card card, PointerEventData eventData)
        {
            if (_discardSystem.HoverCard == null)
            {
                return;
            }
            
            TryInvokeDropHandler();
            RequestOnEndDrag?.Invoke();
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedObject = eventData.pointerDrag;
            if (droppedObject == null)
            {
                return;
            }
            
            var card = droppedObject.GetComponent<Card>();
            if (card == null)
            {
                return;
            }

            if (card.CardType == CardType.Hand)
            {
                _dropSystem.OnDroppedHandToDiscard(card);    
            }
        }

        private void MoveHoverCardToMousePosition(Vector2 position)
        {
            _discardSystem.HoverCard.transform.position = position;
        }

        private bool TryInvokeDropHandler()
        {
            var handHit  = CheckField(out var eventData);
            if (handHit == null)
            {
                return false;
            }
            
            var handEventSystem = handHit.Value.gameObject.GetComponent<HandEventSystem>();
            if (handEventSystem == null)
            {
                return false;
            }
            
            ExecuteEvents.Execute(handHit.Value.gameObject, eventData, ExecuteEvents.dropHandler);
            return true;
        }

        private RaycastResult? CheckField(out PointerEventData eventData)
        {
            eventData = new(EventSystem.current)
            {
                position = Input.mousePosition,
                pointerDrag = _discardSystem.HoverCard.gameObject
            };
            
            var rayHits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rayHits);

            foreach (var hit in rayHits)
            {
                var card = hit.gameObject.GetComponent<Card>();
                if (card != null && _container.IsExist(card) && _discardSystem.HoverCard != card)
                {
                    return hit;
                }
                
                var dropHandler = hit.gameObject.GetComponent<IDropHandler>();
                if (dropHandler != null)
                {
                    return hit;
                }
            }

            return null;
        }
    }
}