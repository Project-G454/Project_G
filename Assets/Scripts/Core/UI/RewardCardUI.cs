using System;
using Cards;
using Cards.Data;
using Core.Managers;
using DG.Tweening;
using Shop.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Reward {
    public class RewardCard: MonoBehaviour, IPointerClickHandler {
        [SerializeField] private RectTransform _layout;
        public CardData cardData;
        private Card _card;
        public GameObject cardObj;
        private Action<CardData> _onCardSelectedCallBack;

        public void Init(CardData data, Action<CardData> onCardSelectedCallBack) {
            cardData = data;

            _card = new Card(data);
            CardBehaviour cb = cardObj.GetComponent<CardBehaviour>();
            cb.Init(cardObj, _card);

            _onCardSelectedCallBack = onCardSelectedCallBack;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                _onCardSelectedCallBack?.Invoke(cardData);
            }
            else if (eventData.button == PointerEventData.InputButton.Right) {
                GlobalUIManager.Instance.cardActiveUI.Show(_card);
            }
        }

        public void AnimateFocus(bool isCurrent) {
            float targetScale = isCurrent ? 1.2f : 1f;
            float animationDuration = 0.2f;

            _layout.DOScaleX(targetScale, animationDuration);
            _layout.DOScaleY(targetScale, animationDuration);

            // 使用 DOTween 動畫調整尺寸
            // DOTween.To(() => _layout.preferredWidth, x => _layout.preferredWidth = x, targetWidth, animationDuration);
            // DOTween.To(() => _layout.preferredHeight, x => _layout.preferredHeight = x, targetHeight, animationDuration);
        }
    }
}
