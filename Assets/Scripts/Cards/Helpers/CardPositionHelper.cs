using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards.Helpers {
    class CardPositionHelper {
        private const float _CARD_GAP = 5f;
        public static List<Vector3> CalcCardPosition(Transform cardParent, List<GameObject> cards) {
            List<Vector3> cardPositions = new();
            if (cards.Count() <= 0) return cardPositions;
            
            Vector3 center = cardParent.position;
            RectTransform cardRect = cards[0].GetComponent<RectTransform>();
            float cardWidth = cardRect.rect.width;
            float cardHeight = cardRect.rect.height;
            float totalWidth = cardWidth * cards.Count() + _CARD_GAP * (cards.Count() - 1);
            float offsetX = (totalWidth - cardWidth) * 0.5f;

            for (int i=0; i<cards.Count(); i++) {
                cardPositions.Add(new Vector3((cardWidth + _CARD_GAP) * i + center.x - offsetX, cardHeight * 0.6f, 0f));
            }

            return cardPositions;
        }
    }
}
