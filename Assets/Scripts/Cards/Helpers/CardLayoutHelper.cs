using System;
using Core.Managers.Cards;
using UnityEngine;

namespace Cards.Helpers {
    public static class CardLayoutHelper {
        private const int FRAME_COUNT = 3;

        public static Sprite GetFrameSprite(int id) {
            return Array.Find(CardManager.cardAssets.ToArray(), cardSprite => cardSprite.name == "Card_Frame_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite GetCostSprite(int id) {
            return Array.Find(CardManager.cardAssets.ToArray(), cardSprite => cardSprite.name == "Card_Cost_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite GetTitleSprite(int id) {
            return Array.Find(CardManager.cardAssets.ToArray(), cardSprite => cardSprite.name == "Card_Title_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite GetTypeSprite(string type) {
            return Array.Find(CardManager.cardAssets.ToArray(), cardSprite => cardSprite.name == "Card_Type_" + type);
        }
    }
}
