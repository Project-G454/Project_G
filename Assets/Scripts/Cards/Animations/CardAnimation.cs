using System;
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

        public static List<Vector3> ResetCardPos(Transform cardParent, List<GameObject> cards, bool returnPos=false) {
            List<Vector3> cardPositions = CardPositionHelper.CalcCardPosition(cardParent, cards);
            for (int i=0; i<cards.Count(); i++) {
                RectTransform cardRT = cards[i].GetComponent<RectTransform>();
                cardRT.DOMove(cardPositions[i], 0.2f);
            }
            return returnPos? cardPositions : null;
        }

        public static void Dodge(Transform cardParent, int targetIdx, List<GameObject> cards) {
            for (int i=0; i<cards.Count(); i++) {
                if (i == targetIdx) continue;

                List<Vector3> cardPositions = CardPositionHelper.CalcCardPosition(cardParent, cards);
                RectTransform cardRT = cards[i].GetComponent<RectTransform>();
                float offset = 20f * Math.Max(0, 3 - Math.Abs(targetIdx - i));

                if (i < targetIdx) cardRT.DOMoveX(cardPositions[i].x - offset, 0.2f);
                else cardRT.DOMoveX(cardPositions[i].x + offset, 0.2f);
            }
        }

        public static void ZoomIn(GameObject cardObj, float scaleFactor=1.2f, float duration=1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOKill();
            cardTransform.DOScale(view.GetInitialScale() * scaleFactor, duration);
        }

        public static void ZoomOut(GameObject cardObj, float duration=1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOKill();
            cardTransform.DOScale(view.GetInitialScale(), duration);
        }

        public static void MoveTo(GameObject cardObj, Vector3 offset, float duration=1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOKill();
            cardTransform.DOMove(view.GetInitialPosition() + offset, duration);
        }

        public static void SetAlpha(GameObject cardObj, float alpha=0.5f) {
            CanvasGroup cg = cardObj.GetComponent<CanvasGroup>();
            if (cg == null) return;

            cg.alpha = alpha;
        }
    }
}
