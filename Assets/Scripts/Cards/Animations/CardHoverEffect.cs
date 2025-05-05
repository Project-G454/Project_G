using System.Collections.Generic;
using Core.Managers;
using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cards.Helpers;

namespace Cards.Animations {
    public class CardHoverEffect: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler {
        public float scaleUp;
        public Vector3 offset;
        public float duration;
        public Vector3 originalScale;
        public Vector3 originalPosition;
        private bool _isHovered = false;
        private bool _isDragging = false;
        private RectTransform _rectTransform;
        private CardBehaviour _cardBehaviour;
        private List<CanvasGroup> _otherCards;
        private int _originalSiblingIdx;
        private DescriptionManager _descriptionManager;

        public void Init() {
            _rectTransform = GetComponent<RectTransform>();
            _cardBehaviour = GetComponent<CardBehaviour>();
            _descriptionManager = DescriptionManager.Instance;

            int idx = CardManager.cardList.IndexOf(gameObject);
            originalPosition = CardPositionHelper.CalcCardPosition(
                transform.parent, 
                CardManager.cardList
            )[idx];
            originalScale = transform.localScale;

            _otherCards = new List<CanvasGroup>();
            foreach (Transform child in transform.parent) {
                if (child == transform) continue;
                var group = child.GetComponent<CanvasGroup>();
                if (group != null) _otherCards.Add(group);
            }
        }

        void Start() {
            Init();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (_isHovered) return;
            _isHovered = true;
            
            // Move layer to top
            _originalSiblingIdx = transform.GetSiblingIndex();
            transform.SetAsLastSibling();

            // zoom in
            transform.DOKill();
            transform.DOScale(originalScale * scaleUp, duration);
            transform.DOMove(originalPosition + offset, duration);
            
            // other card dodge
            int cardIdx = CardManager.cardList.IndexOf(_cardBehaviour.cardObject);
            CardAnimation.Dodge(transform.parent, cardIdx, CardManager.cardList);

            // show tooltips
            if (_descriptionManager == null || _cardBehaviour == null) return;
            _descriptionManager.ShowDescriptions(_cardBehaviour.card.desctiptionIds);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (!_isHovered) return;
            _isHovered = false;

            if (_isDragging) return;
            transform.SetSiblingIndex(_originalSiblingIdx);
            EndAnimation();

            _descriptionManager?.HideAll();
        }

        private void EndAnimation() {
            transform.DOScale(originalScale, duration);

            CardAnimation.ResetCardPos(transform.parent, CardManager.cardList);

            if (!_isHovered) _descriptionManager?.HideAll();
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
