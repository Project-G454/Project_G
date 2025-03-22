using UnityEngine;

namespace Core.Helpers {
    public static class TextureHelper {
        public static Texture2D OverlayTextures(Texture2D baseTexture, Texture2D overlayTexture, bool pixelPerfact = false) {
            int width = baseTexture.width;
            int height = baseTexture.height;

            Texture2D res = new Texture2D(width, height, TextureFormat.RGBA32, false);
            res.SetPixels(baseTexture.GetPixels());

            if (pixelPerfact) {
                res.filterMode = FilterMode.Point;          // 確保使用 nearest-neighbor 避免模糊
                res.wrapMode = TextureWrapMode.Clamp;       // 防止邊緣問題
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Color baseColor = baseTexture.GetPixel(x, y);
                    Color overlayColor = overlayTexture.GetPixel(x, y);
                    // 考慮 overlay 的透明度
                    Color resColor = Color.Lerp(baseColor, overlayColor, overlayColor.a);
                    res.SetPixel(x, y, resColor);
                }
            }
            res.Apply();

            return res;
        }

        public static Texture2D SpriteToTexture2D(Sprite sprite) {
            if (sprite == null) return null;

            
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            
            Color[] pixels = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
