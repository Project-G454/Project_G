using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Helpers;
using Core.Entities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Animations {
    class CardAnimation {

        public static void StopAllAnimation(GameObject cardObj) {
            DOTween.Kill(cardObj);
        }

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

        public static void Dodge(Transform cardParent, int targetIdx, List<GameObject> cards, float gap=20f) {
            for (int i=0; i<cards.Count(); i++) {
                if (i == targetIdx) continue;

                List<Vector3> cardPositions = CardPositionHelper.CalcCardPosition(cardParent, cards);
                RectTransform cardRT = cards[i].GetComponent<RectTransform>();
                float offset = gap * Math.Max(0, 3 - Math.Abs(targetIdx - i));

                if (i < targetIdx) cardRT.DOMoveX(cardPositions[i].x - offset, 0.2f);
                else cardRT.DOMoveX(cardPositions[i].x + offset, 0.2f);
            }
        }

        public static void ZoomIn(GameObject cardObj, float scaleFactor=1.2f, float duration=0.1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOScale(view.GetInitialScale() * scaleFactor, duration);
        }

        public static void ZoomOut(GameObject cardObj, float duration=0.1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOScale(view.GetInitialScale(), duration);
        }

        public static void MoveTo(GameObject cardObj, Vector3 position, float duration=0.1f) {
            CardView view = cardObj.GetComponent<CardView>();
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null || view == null) return;

            cardTransform.DOMove(position, duration);
        }
        
        public static void LocalMoveTo(RectTransform rt, Vector3 position, float duration=0.1f) {
            if (rt == null) return;

            rt.DOAnchorPos(position, duration);
        }

        public static void MoveToPointer(GameObject cardObj, PointerEventData eventData, float duration = 0.1f) {
            Canvas canvas = cardObj.GetComponent<Canvas>();
            RectTransform rt = cardObj.GetComponent<RectTransform>();

            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out worldPos
            );

            rt.DOMove(worldPos, duration);
        }

        public static void MoveToEntity(GameObject cardObj, int targetId) {
            Canvas canvas = cardObj.GetComponentInParent<Canvas>();
            RectTransform rt = cardObj.GetComponent<RectTransform>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            GameObject entityObj = EntityManager.Instance.GetEntityObject((int)targetId);

            // 取得 entity 的世界座標
            Vector3 entityWorldPos = entityObj.transform.position;

            // 把 entity 的世界座標映射到 UI 座標
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, entityWorldPos);

            // 計算 UI 投影出的世界座標
            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out worldPos);

            rt.position = worldPos;
        }

        public static void SetAlpha(GameObject cardObj, float alpha=0.5f) {
            CanvasGroup cg = cardObj.GetComponent<CanvasGroup>();
            if (cg == null) return;

            cg.alpha = alpha;
        }

        public static void SendToFront(GameObject cardObj) {
            Transform cardTransform = cardObj.GetComponent<Transform>();
            if (cardTransform == null) return;

            cardTransform.SetAsLastSibling();
        }

        public static void ResetSibling(GameObject cardObj) {
            Transform cardTransform = cardObj.GetComponent<Transform>();
            CardView view = cardObj.GetComponent<CardView>();
            if (cardTransform == null || view == null) return;

            cardTransform.SetSiblingIndex(view.GetInitialSiblingIdx());
        }
    }
}
