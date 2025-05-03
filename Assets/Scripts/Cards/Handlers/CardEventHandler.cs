using Cards.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    class CardEventHandler: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private bool _isPointerOver = false;
        private bool _isClicked = false;
        private bool _isDragging = false;
        private bool _isTargeting = false;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            _isClicked = true;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            _isPointerOver = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            _isPointerOver = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            _isDragging = true;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            throw new System.NotImplementedException();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            throw new System.NotImplementedException();
        }

        public bool IsPointerEnter() => _isPointerOver;
        public bool IsPointerExit() => !_isPointerOver;
        public bool IsDragging() => _isDragging;

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
            cg.alpha = 1f;

            // UI to World Raytrace
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var receiver = hit.collider.GetComponent<UseCardReceiver>();
                if (receiver != null) return true;
            }
            return false;
        }
    }
}
