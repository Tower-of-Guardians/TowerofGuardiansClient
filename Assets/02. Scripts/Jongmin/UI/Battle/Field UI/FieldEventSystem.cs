using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jongmin
{
    public class FieldEventSystem : MonoBehaviour
    {
        private FieldSystem _fieldSystem;
        private CardDropSystem _dropSystem;
        private CardContainer _container;
        
        public event Action<Card, FieldType> RequestOnBeginDrag;
        public event Action<Card, FieldType, Vector2> RequestSwapInSameField;
        public event Action<FieldType> RequestOnEndDrag;
        public event Action<FieldType> RequestMoveHoverCardToOpposite;
        
        public void Construct(FieldSystem fieldSystem, CardDropSystem dropSystem, CardContainer container)
        {
            _fieldSystem = fieldSystem;
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

        public void HandleOnBeginDrag(Card card, PointerEventData eventData)
        {
            var fieldType = GetFieldType(card);
            
            RequestOnBeginDrag?.Invoke(card, fieldType);
        }

        public void HandleOnDrag(Card card, PointerEventData eventData)
        {
            if (_fieldSystem.HoverCard == null)
            {
                return;
            }
            
            MoveHoverCardToMousePosition(eventData.position);

            if (!TryGetFieldCard(out var fieldCard))
            {
                return;
            }

            if (_container.IsExist(fieldCard))
            {
                RequestSwapInSameField?.Invoke(fieldCard, GetFieldType(fieldCard), eventData.position);
            }
        }

        public void HandleOnEndDrag(Card card, PointerEventData eventData)
        {
            if (_fieldSystem.HoverCard == null)
            {
                return;
            }

            var fieldType = GetFieldType(card);
            
            TryInvokeDropHandler();
            RequestOnEndDrag?.Invoke(fieldType);
        }

        public bool TryInsertInOppositeFieldWithField()
        {
            var fieldHit = CheckField(out _);
            if (fieldHit == null)
            {
                return false;
            }
            
            var fieldEventSystem = fieldHit?.gameObject.GetComponent<FieldEventSystem>();
            if (fieldEventSystem == null || fieldEventSystem == this)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool TryInsertInOppositeFieldWithCard()
        {
            var cardHit = CheckField(out _);
            if (cardHit == null)
            {
                return false;
            }
            
            var card = cardHit?.gameObject.GetComponent<Card>();
            if (card == null || card.CardType is not (CardType.AtkField or CardType.DefField) || !_container.IsExist(card))
            {
                return false;
            }
            else
            {
                var fieldType = GetFieldType(card);
                RequestMoveHoverCardToOpposite?.Invoke(fieldType);
                return true;
            }
        }

        private FieldType GetFieldType(Card card)
        {
            return card.CardType switch
            {
                CardType.AtkField => FieldType.Attack,
                CardType.DefField => FieldType.Defense
            };
        }

        private void MoveHoverCardToMousePosition(Vector2 position)
        {
            _fieldSystem.HoverCard.transform.position = position;
        }

        private RaycastResult? CheckField(out PointerEventData eventData)
        {
            eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition,
                pointerDrag = _fieldSystem.HoverCard.gameObject
            };

            var rayHits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rayHits);

            foreach (var hit in rayHits)
            {
                var fieldEventSystem = hit.gameObject.GetComponent<FieldEventSystem>();
                if (fieldEventSystem != null && fieldEventSystem != this)
                {
                    return hit;
                }
                
                var card = hit.gameObject.GetComponent<Card>();
                if (card != null && _fieldSystem.HoverCard != card)
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

        private bool TryGetFieldCard(out Card card)
        {
            var cardHit = CheckField(out _);
            if (cardHit == null)
            {
                card = null;
                return false;
            }

            card = cardHit.Value.gameObject.GetComponent<Card>();
            if (card == null || card.CardType is not (CardType.AtkField or CardType.DefField))
            {
                card = null;
                return false;
            }

            return true;
        }

        private bool TryInvokeDropHandler()
        {
            var handHit = CheckField(out var eventData);
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
    }
}