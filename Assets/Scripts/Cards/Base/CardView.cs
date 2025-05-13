using Cards.Helpers;
using Core.Loaders.Cards;
using Core.Managers.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards {
    public class CardView: MonoBehaviour {
        public TMP_Text title;
        public TMP_Text cost;
        public TMP_Text description;
        public Image background;
        public Image frame;
        public Image costBackground;
        public Image titleBackground;
        public Image typeBackground;
        public Card card;
        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private int _originalSiblingIdx;

        public void SetCardView(Card card) {
            this.card = card;
            title.text = card.cardName;
            cost.text = card.cost.ToString();
            description.text = card.description;

            background.sprite = CardDataLoader.LoadBackground(card.id);
            frame.sprite = CardDataLoader.LoadFrame((int)card.rarity);
            costBackground.sprite = CardLayoutHelper.GetCostSprite((int)card.rarity);
            titleBackground.sprite = CardLayoutHelper.GetTitleSprite((int)card.rarity);
            typeBackground.sprite = CardLayoutHelper.GetTypeSprite(card.type.ToString());

            // _originalPosition = transform.position;
            // _originalScale = transform.lossyScale;
            // _originalSiblingIdx = transform.GetSiblingIndex();
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
