using System.Collections;
using Cards;
using Core.Handlers;
using Core.Managers;
using Descriptions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI {
    public class CardActiveUI: MonoBehaviour, IPointerClickHandler {
        public CardView cardView;
        public DescriptionView cardDesc;
        public Transform effectParent;
        public RectTransform layout;
        private bool _isShow = false;
        public TMP_Text hint;

        public void Update() {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            LayoutRebuilder.ForceRebuildLayoutImmediate(effectParent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardDesc.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void Show(Card card) {
            if (_isShow) return;

            _ResetEffectView();
            _SetCardView(card);
            _SetCardDescription(card);
            _SetEffectView(card);
            hint.alpha = 1f;
            hint.DOFade(0f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            gameObject.SetActive(true);
            _isShow = true;
        }

        public void Hide() {
            gameObject.SetActive(false);
            _ResetEffectView();
            hint.DOKill();
            _isShow = false;
        }

        private void _SetCardView(Card card) {
            if (cardView == null) return;
            cardView.SetCardView(card);
        }

        private void _SetCardDescription(Card card) {
            if (cardDesc == null) return;
            Description description = new Description(card.cardName, card.description);
            cardDesc.SetView(description);
        }

        private void _SetEffectView(Card card) {
            foreach (int id in card.desctiptionIds) {
                DescriptionBehaviour description = DescriptionManager.Instance.GetById(id);
                GameObject descObj = Instantiate(description.obj, effectParent);
                descObj.transform.SetParent(effectParent);
                descObj.GetComponent<ForceUpdateFitter>().Fit();
            }
        }

        private void _ResetEffectView() {
            for (int i = effectParent.childCount - 1; i >= 0; i--) {
                Destroy(effectParent.GetChild(i).gameObject);
            }
        }

        private IEnumerator _ForceRebuildNextFrame()
        {
            yield return null; // 等待一幀讓 TextMeshPro 更新尺寸
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            Hide();
        }
    }
}
