using UnityEngine;

public static class PrefabRenderer {
    public static Sprite RenderHeadToSprite(GameObject prefab, Camera renderCam, int width = 256, int height = 256)
    {
        renderCam.gameObject.SetActive(true);

        // 1. 建立暫時物件
        GameObject instance = Object.Instantiate(prefab);
        int layer = LayerMask.NameToLayer("HeadCapture");
        SetLayerRecursively(instance, layer);
        instance.SetActive(true);

        // 2. 設定攝影機
        renderCam.orthographic = true;
        renderCam.orthographicSize = 0.5f;
        instance.transform.position = renderCam.transform.position + renderCam.transform.forward * 2 + new Vector3(0.1f, -0.2f, 0f);
        instance.transform.rotation = Quaternion.identity;

        // 3. 建立 RenderTexture + 清空畫面
        RenderTexture rt = new RenderTexture(width, height, 24);
        renderCam.targetTexture = rt;
        RenderTexture.active = rt;
        renderCam.Render();

        // 4. 擷取圖像
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // 5. 清理
        renderCam.targetTexture = null;
        RenderTexture.active = null;
        Graphics.SetRenderTarget(null);
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(instance);
        renderCam.gameObject.SetActive(false);

        // 6. 回傳 Sprite
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private static void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
