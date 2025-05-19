using Cards.Categories;
using Cards.Helpers;
using Core.Loaders.Cards;
using Core.Managers;
using Core.Managers.Cards;
using Effects;
using Effects.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards {
    public class CardView: MonoBehaviour {
        public TMP_Text title;
        public TMP_Text cost;
        public Image background;
        public Image frame;
        public Image titleBackground;
        public GameObject effectPrefab;
        public Transform effectParent;
        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private int _originalSiblingIdx;

        public void SetCardView(Card card) {
            card.ApplyView(this);
        }

        public void CreateEffectDisplay(Sprite icon, int rounds) {
            GameObject effect = Instantiate(effectPrefab, effectParent);
            effect.transform.SetParent(effectParent);

            CardEffectDisplay display = effect.GetComponent<CardEffectDisplay>();
            display.SetDisplay(icon, rounds);
        }

        public Vector3 GetInitialPosition() => _originalPosition;
        public Vector3 GetInitialScale() => _originalScale;
        public int GetInitialSiblingIdx() => _originalSiblingIdx;

        public void SetInitialState(Vector3 position, Vector3 scale, int siblingIdx) {
            _originalPosition = position;
            _originalScale = scale;
            _originalSiblingIdx = siblingIdx;
        }

        public void RecordInitialState() {
            int idx = CardManager.cardList.IndexOf(gameObject);
            Vector3 originalPosition = CardPositionHelper.CalcCardPosition(
                transform.parent, 
                CardManager.cardList
            )[idx];
            Vector3 originalScale = transform.localScale;
            SetInitialState(originalPosition, originalScale, transform.GetSiblingIndex());
        }
    }
}
