using System;
using Core.Helpers;
using Core.Managers.UI;
using UnityEngine;

namespace Cards.Helpers {
    public static class CardLayoutHelper {
        private static CardSpriteManager _cardSpriteManager;
        private static CardSpriteManager CardSpriteManager => _cardSpriteManager ??= CardSpriteManager.Instance;
        private static Sprite[] cardSprites  => CardSpriteManager.cardSprites;
        private const int BG_COUNT = 7;
        private const int FRAME_COUNT = 3;
        public static Sprite getCardSprite(int bgId, int frameId) {
            Sprite cardBackground = getBackgroundSprite(bgId);
            Sprite cardFrame = getFrameSprite(frameId);

            Texture2D backgroundTexture = TextureHelper.SpriteToTexture2D(cardBackground);
            Texture2D frameTexture = TextureHelper.SpriteToTexture2D(cardFrame);
            Texture2D layoutTexture = TextureHelper.OverlayTextures(backgroundTexture, frameTexture, true);

            return Sprite.Create(
                layoutTexture,
                new Rect(0, 0, layoutTexture.width, layoutTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        public static Sprite getBackgroundSprite(int id) {
            return System.Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Background_" + Math.Clamp(id, 1, BG_COUNT).ToString());
        }

        public static Sprite getFrameSprite(int id) {
            return System.Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Frame_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getCostSprite(int id) {
            return System.Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Cost_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getTitleSprite(int id) {
            return System.Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Title_" + Math.Clamp(id, 1, FRAME_COUNT).ToString());
        }

        public static Sprite getTypeSprite(string type) {
            return System.Array.Find(cardSprites, cardSprite => cardSprite.name == "Card_Type_" + type);
        }
    }
}
