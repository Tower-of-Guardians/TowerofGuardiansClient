using System;
using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jongmin
{
    [RequireComponent(typeof(JxEmptyGraphic))]
    public class CardPointer : MonoBehaviour, 
                               IPointerEnterHandler, 
                               IPointerExitHandler, 
                               IPointerClickHandler, 
                               IPointerDownHandler, 
                               IPointerUpHandler, 
                               IBeginDragHandler, 
                               IDragHandler, 
                               IEndDragHandler
    {
        public event Action<PointerEventData> OnPointerEntered;
        public event Action<PointerEventData> OnPointerExited;
        public event Action<PointerEventData> OnPointerClicked;
        public event Action<PointerEventData> OnPointerDowned;
        public event Action<PointerEventData> OnPointerUpped;
        public event Action<PointerEventData> OnBeginDragged;
        public event Action<PointerEventData> OnDragged;
        public event Action<PointerEventData> OnEndDragged;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEntered?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExited?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClicked?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDowned?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpped?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragged?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragged?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragged?.Invoke(eventData);
        }
    }
}