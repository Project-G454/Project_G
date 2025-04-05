using System.Collections.Generic;
using Core.Helpers;
using Core.Managers;
using Core.Managers.Cards;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards.Animations {
    public class CardHoverEffect: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler {
        public float scaleUp;
        public Vector3 offset;
        public float duration;
        public Vector3 originalScale;
        public Vector3 originalPosition;
        public Vector2 originalSize;
        public Vector3 originalWorldPosition;
        private bool _isHovered = false;
        private bool _isDragging = false;
        private RectTransform _rectTransform;
        private CardBehaviour _cardBehaviour;
        private CardPositionManager _cardPositionManager;
        private CardAnimator _animator;
        private Transform _parent;
        private List<CanvasGroup> _otherCards;
        private int _originalSiblingIdx;
        private DescriptionManager _descriptionManager;

        public void Init() {
            originalPosition = _rectTransform.anchoredPosition;
            originalSize = _rectTransform.rect.size;
            originalWorldPosition = _rectTransform.position;
        }

        void Start() {
            originalScale = transform.localScale;
            _rectTransform = GetComponent<RectTransform>();
            _cardBehaviour = GetComponent<CardBehaviour>();
            _cardPositionManager = CardPositionManager.Instance;
            _animator = GetComponent<CardAnimator>();
            _descriptionManager = DescriptionManager.Instance;

            _parent = transform.parent;
            _otherCards = new List<CanvasGroup>();
            foreach (Transform child in _parent) {
                if (child == transform) continue;
                var group = child.GetComponent<CanvasGroup>();
                if (group != null) _otherCards.Add(group);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (_isHovered) return;
            _isHovered = true;
            _originalSiblingIdx = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
            StartAnimation();

            if (_descriptionManager == null || _cardBehaviour == null) return;

            Vector3 worldPos = originalWorldPosition + new Vector3(0, 0, 0);

            _descriptionManager.ShowDescriptions(_cardBehaviour.card.desctiptionIds, _rectTransform);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (!_isHovered) return;
            _isHovered = false;
            if (_isDragging) return;
            transform.SetSiblingIndex(_originalSiblingIdx);
            EndAnimation();

            _descriptionManager?.HideAll();
        }

        private void StartAnimation() {
            _animator.MoveTo(originalPosition + offset);
            _animator.ScaleTo(originalScale * scaleUp);

            if (_cardPositionManager != null) {
                int cardIdx = CardManager.cardList.IndexOf(_cardBehaviour.cardObject);
                _cardPositionManager.RepositionCard(CardManager.cardList, cardIdx);
            }
        }

        private void EndAnimation() {

            _animator.MoveTo(originalPosition);
            _animator.ScaleTo(originalScale);

            if (_cardPositionManager != null)
                _cardPositionManager.ResetCardPos(CardManager.cardList);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            _isDragging = true;

            foreach (CanvasGroup group in _otherCards) {
                if (group != null) group.blocksRaycasts = false;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            _isDragging = false;
            transform.SetSiblingIndex(_originalSiblingIdx);

            foreach (CanvasGroup group in _otherCards) {
                if (group != null) group.blocksRaycasts = true;
            }

            EndAnimation();
        }
    }
}
