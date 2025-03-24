using System.Collections.Generic;
using Cards;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using Entities.Categories;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler {
    public float scaleUp = 1.2f;
    public Vector3 offset = new Vector3(0f, 50f, 0f);
    public float duration = 0.1f;
    public Vector3 originalScale;
    public Vector3 originalPosition;
    private bool _isHovered = false;
    private bool _isDragging = false;
    private RectTransform _rectTransform;
    private CardBehaviour _cardBehaviour;
    private CardPositionManager _cardPositionManager;
    private CardAnimator _animator;
    private Transform _parent;
    private List<CanvasGroup> _otherCards;
    private int _originalSiblingIdx;

    public void Init() {
        originalPosition = _rectTransform.anchoredPosition;
    }

    void Start() {
        originalScale = transform.localScale;
        _rectTransform = GetComponent<RectTransform>();
        _cardBehaviour = GetComponent<CardBehaviour>();
        _cardPositionManager = CardPositionManager.Instance;
        _animator = GetComponent<CardAnimator>();

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
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        if (!_isHovered) return;
        _isHovered = false;
        if (_isDragging) return;
        transform.SetSiblingIndex(_originalSiblingIdx);
        EndAnimation();
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
