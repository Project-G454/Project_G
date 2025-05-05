using Cards.Helpers;
using Core.Loaders.Cards;
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

        public void SetCardView(Card card) {
            this.card = card;
            title.text = card.cardName;
            cost.text = card.cost.ToString();
            description.text = card.description;

            background.sprite = CardDataLoader.LoadBackground(card.id);
            frame.sprite = CardLayoutHelper.GetFrameSprite((int)card.rarity);
            costBackground.sprite = CardLayoutHelper.GetCostSprite((int)card.rarity);
            titleBackground.sprite = CardLayoutHelper.GetTitleSprite((int)card.rarity);
            typeBackground.sprite = CardLayoutHelper.GetTypeSprite(card.type.ToString());
        }
    }
}
