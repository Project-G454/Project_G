using Cards.Data;
using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    class CardEventHandler: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private bool _isPointerOver = false;
        private bool _isClicked = false;
        private bool _isDragging = false;
        private PointerEventData _eventData;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (!_isDragging) _isClicked = true;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            _isPointerOver = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            _isPointerOver = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            _eventData = eventData;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            _isClicked = false;
            _isDragging = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
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
                var receiver = hit.collider.GetComponent<UseCardReceiver>();
                if (receiver != null) return true;
            }
            return false;
        }

        public int GetHoveringCardIdx() {
            for (int i=0; i<CardManager.cardList.Count; i++) {
                GameObject cardObj = CardManager.cardList[i];
                CardStateHandler stateHandler = cardObj.GetComponent<CardStateHandler>();
                CardState state = stateHandler.GetState();
                if (_IsHoverState(state)) return i;
            }
            return -1;
        }

        public bool IsAnyCardHovering() {
            for (int i=0; i<CardManager.cardList.Count; i++) {
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
                state == CardState.Targeting
            );
        }
    }
}
