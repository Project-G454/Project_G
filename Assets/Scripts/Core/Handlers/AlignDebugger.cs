using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SuperAlignDebugger : MonoBehaviour
{
    public RectTransform reference; // 卡片
    public RectTransform target;    // 說明框

    public Color referenceColor = Color.green;
    public Color targetColor = Color.yellow;

    private void OnDrawGizmos()
    {
        if (reference == null || target == null) return;

        // 畫出 Reference Pivot 位置
        DrawPivot(reference, referenceColor);
        DrawRect(reference, referenceColor);

        // 畫出 Target Pivot 位置
        DrawPivot(target, targetColor);
        DrawRect(target, targetColor);

        // 畫連線
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(reference.position, target.position);
    }

    private void DrawPivot(RectTransform rect, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(rect.position, 5f);
    }

    private void DrawRect(RectTransform rect, Color color)
    {
        Gizmos.color = color;
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
    }
}
