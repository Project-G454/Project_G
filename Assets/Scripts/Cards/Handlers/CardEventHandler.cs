using Cards.Data;
using Cards.Helpers;
using Core.Managers.Cards;
using Systems.Interactions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    class CardEventHandler: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private bool _lock = false;
        private bool _isPointerOver = false;
        private bool _isClicked = false;
        private bool _isDragging = false;
        private PointerEventData _eventData;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (_lock) return;
            if (!_isDragging) _isClicked = true;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (_lock) return;
            _isPointerOver = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (_lock) return;
            _isPointerOver = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (_lock) return;
            _eventData = eventData;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if (_lock) return;
            _isClicked = false;
            _isDragging = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (_lock) return;
            _isDragging = false;
        }

        public bool IsPointerEnter() => _isPointerOver;
        public bool IsPointerExit() => !_isPointerOver;
        public bool IsDragging() => _isDragging;
        public PointerEventData GetEventData() => _eventData;

        public bool IsClicked() {
            if (_isClicked) {
                _isClicked = false;
                return true;
            }
            return false;
        }

        public bool IsPointingAtTarget() {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cg == null) return false;

            cg.blocksRaycasts = true;

            // UI to World Raytrace
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                Receiver receiver = hit.collider.GetComponent<Receiver>();
                return receiver != null && receiver.HasReceiver(ReceiverType.Card);
            }
            return false;
        }

        public int GetHoveringCardIdx() {
            for (int i = 0; i < CardManager.cardList.Count; i++) {
                GameObject cardObj = CardManager.cardList[i];
                CardStateHandler stateHandler = cardObj.GetComponent<CardStateHandler>();
                CardState state = stateHandler.GetState();
                if (_IsHoverState(state)) return i;
            }
            return -1;
        }

        public bool IsAnyCardHovering() {
            for (int i = 0; i < CardManager.cardList.Count; i++) {
                GameObject cardObj = CardManager.cardList[i];
                CardStateHandler stateHandler = cardObj.GetComponent<CardStateHandler>();
                CardState state = stateHandler.GetState();
                if (_IsHoverState(state)) return true;
            }
            return false;
        }

        private bool _IsHoverState(CardState state) {
            return (
                state == CardState.Hover ||
                state == CardState.Dragging ||
                state == CardState.Active ||
                state == CardState.Targeting
            );
        }

        public void Lock() {
            _lock = true;
        }

        public void Unlock() {
            _lock = false;
        }
    }
}
