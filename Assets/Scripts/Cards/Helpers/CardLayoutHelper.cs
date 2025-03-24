using System;
using Core.Managers.UI;
using UnityEngine;

namespace Cards.Helpers {
    public static class CardLayoutHelper {
        private static CardSpriteManager _cardSpriteManager;
        private static CardSpriteManager CardSpriteManager => _cardSpriteManager ??= CardSpriteManager.Instance;
        private static Sprite[] cardSprites => CardSpriteManager.cardSprites;
        private const int BG_COUNT = 7;
        private const int FRAME_COUNT = 3;

        public static Sprite getBackgroundSprite(int id) {
            // return Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Background_" + Math.Clamp(id, 1, BG_COUNT).ToString());
            return LoadImage(id % 15 + 1);
        }

        public static Sprite getFrameSprite(int id) {
            return Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Frame_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getCostSprite(int id) {
            return Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Cost_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getTitleSprite(int id) {
            return Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Title_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getTypeSprite(string type) {
            return Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Type_" + type);
        }

        public static Sprite LoadImage(int id) {
            string filePath = "Cards/Backgrounds/Card_Background_" + id.ToString();

            Sprite sprite = Resources.Load<Sprite>(filePath);
            if (sprite != null) {
                return sprite;
            }
            else {
                Debug.LogError("Load image failed.");
                return null;
            }
        }
    }
}
