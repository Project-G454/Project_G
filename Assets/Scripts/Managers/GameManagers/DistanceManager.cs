using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour, IManager
{
    public static DistanceManager Instance;

    [Header("Settings")]
    public int maxDistance = 5;

    [Header("References")]
    public GameObject highlightTilePrefab;
    public Transform highlightParent;
    public Text warningText;

    [Header("Optional")]
    public Vector3 highlightOffset = Vector3.zero; // 👉 預設偏移為 0，可手動調整

    private List<GameObject> currentHighlights = new();

    void IManager.Init() {
        warningText?.gameObject.SetActive(false);
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowReachableTiles(Vector2Int origin)
    {
        ClearHighlights();

        for (int dx = -maxDistance; dx <= maxDistance; dx++)
        {
            for (int dy = -maxDistance; dy <= maxDistance; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist <= maxDistance)
                {
                    Vector2Int tilePos = origin + new Vector2Int(dx, dy);
                    Vector3 worldPos = new Vector3(tilePos.x, tilePos.y, 0) + highlightOffset;

                    GameObject highlight = Instantiate(highlightTilePrefab, worldPos, Quaternion.identity);
                    highlight.transform.SetParent(highlightParent);

                    // 自動設定 Sorting Order 讓格子蓋在地圖上
                    SpriteRenderer sr = highlight.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = 10;
                    }

                    currentHighlights.Add(highlight);
                }
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var obj in currentHighlights)
        {
            if (obj != null) Destroy(obj);
        }
        currentHighlights.Clear();
    }

    public bool IsTileInRange(Vector2Int origin, Vector2Int target)
    {
        int distance = Mathf.Abs(origin.x - target.x) + Mathf.Abs(origin.y - target.y);
        return distance <= maxDistance;
    }

    public void ShowOutOfRangeWarning()
    {
        if (warningText != null)
        {
            warningText.text = "超出可移動範圍！";
            warningText.color = Color.red;
            warningText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), 2f);
        }
    }

    private void HideWarning()
    {
        warningText?.gameObject.SetActive(false);
    }
}
