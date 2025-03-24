using System.Collections;
using Cards.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards {
    public class CardView: MonoBehaviour {
        public int bgId = 0;
        // public int frameId;
        // public SpriteRenderer layoutRenderer;
        public TMP_Text title;
        public TMP_Text cost;
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
            
            background.sprite = CardLayoutHelper.getBackgroundSprite(card.id);
            frame.sprite = CardLayoutHelper.getFrameSprite((int)card.rarity);
            costBackground.sprite = CardLayoutHelper.getCostSprite((int)card.rarity);
            titleBackground.sprite = CardLayoutHelper.getTitleSprite((int)card.rarity);
            typeBackground.sprite = CardLayoutHelper.getTypeSprite(card.type.ToString());
        }

        
    }
}
