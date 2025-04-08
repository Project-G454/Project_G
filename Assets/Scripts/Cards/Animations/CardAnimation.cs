using System.Collections.Generic;
using System.Linq;
using Cards.Helpers;
using DG.Tweening;
using UnityEngine;

namespace Cards.Animations {
    class CardAnimation {
        // 發牌
        public static void Deal(Transform cardParent, List<GameObject> cards) {
            ResetCardPos(cardParent, cards);
        }

        public static void ResetCardPos(Transform cardParent, List<GameObject> cards) {
            List<Vector3> cardPositions = CardPositionHelper.CalcCardPosition(cardParent, cards);
            for (int i=0; i<cards.Count(); i++) {
                RectTransform cardRT = cards[i].GetComponent<RectTransform>();
                cardRT.DOMove(cardPositions[i], 0.2f);
            }
        }

        public static void Dodge(Transform cardParent, int targetIdx, List<GameObject> cards) {
            for (int i=0; i<cards.Count(); i++) {
                if (i == targetIdx) continue;

                List<Vector3> cardPositions = CardPositionHelper.CalcCardPosition(cardParent, cards);
                RectTransform cardRT = cards[i].GetComponent<RectTransform>();

                if (i < targetIdx) cardRT.DOMoveX(cardPositions[i].x - 20f, 0.2f);
                else cardRT.DOMoveX(cardPositions[i].x + 20f, 0.2f);
            }
        }
    }
}
