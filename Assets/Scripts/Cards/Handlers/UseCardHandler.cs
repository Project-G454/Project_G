using Cards.Data;
using Core.Entities;
using Core.Managers.Cards;
using Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    public class UseCardHandler:
        MonoBehaviour,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private CardBehaviour _cardBehaviour;
        private CardManager _cardManager;
        void Init() {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _cardBehaviour = GetComponent<CardBehaviour>();
            _cardManager = CardManager.Instance;
        }

        public void Start() {
            Init();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0.5f;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;

            // UI to World Raytrace
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var receiver = hit.collider.GetComponent<UseCardReceiver>();

                if (receiver == null) return;

                receiver.OnDrop(this.gameObject);
                EntityBehaviour entityBehaviour = receiver.GetComponent<EntityBehaviour>();
                if (entityBehaviour == null) return;

                int targetId = entityBehaviour.entity.entityId;
                _cardManager.UseCard(_cardBehaviour, targetId);
            }
        }

        public void UseCard() {
            var targetId = _GetTargetId();
            if (targetId == null) return;

            Entity target = EntityManager.Instance.GetEntity((int)targetId);
            _cardManager.UseCard(_cardBehaviour, target.entityId);
        }

        private int? _GetTargetId() {
            // UI to World Raytrace
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var receiver = hit.collider.GetComponent<UseCardReceiver>();
                if (receiver == null) return null;

                EntityBehaviour entityBehaviour = receiver.GetComponent<EntityBehaviour>();
                if (entityBehaviour == null) return null;

                int targetId = entityBehaviour.entity.entityId;
                return targetId;
            }
            return null;
        }
    }
}
