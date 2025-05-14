using UnityEngine;

namespace Core.Helpers {
    public static class UIPositionHelper {
        public static void AlignPositionOnly(RectTransform target, RectTransform reference) {
            // 把 target 移到和 reference 一樣的 parent 空間
            target.SetParent(reference.parent, false);

            // 保持原本大小、pivot、layout 不變，只動 position
            target.position = reference.position;
        }

        public static Vector3 GetPivotWorldPosition(RectTransform rect) {
            Vector2 size = rect.rect.size;
            Vector2 pivot = rect.pivot;

            Vector3 localOffset = new Vector3(
                (pivot.x - 0.5f) * size.x * rect.lossyScale.x,
                (pivot.y - 0.5f) * size.y * rect.lossyScale.y,
                0f
            );

            return rect.TransformPoint(localOffset);
        }
    }
}
