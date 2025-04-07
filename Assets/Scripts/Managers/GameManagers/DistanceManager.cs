using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager Instance;

    [Header("Settings")]
    public int maxDistance = 5;

    [Header("References")]
    public Tilemap groundTilemap;
    public Tilemap highlightTilemap;
    public TileBase highlightTile;
    public Text warningText;

    private void Awake()
    {
        Instance = this;
        warningText.gameObject.SetActive(false);
    }

    public void ShowReachableTiles(Vector2Int origin)
    {
        ClearHighlights();

        var reachable = GetTilesInRange(origin, maxDistance);
        foreach (var pos in reachable)
        {
            highlightTilemap.SetTile((Vector3Int)pos, highlightTile);
        }
    }

    public bool IsTileInRange(Vector2Int origin, Vector2Int target)
    {
        int distance = Mathf.Abs(origin.x - target.x) + Mathf.Abs(origin.y - target.y);
        return distance <= maxDistance;
    }

    public void ShowOutOfRangeWarning()
    {
        warningText.text = "超出可移動範圍！";
        warningText.color = Color.red;
        warningText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideWarning));
        Invoke(nameof(HideWarning), 2f); // 自動隱藏
    }

    private void HideWarning()
    {
        warningText.gameObject.SetActive(false);
    }

    private void ClearHighlights()
    {
        highlightTilemap.ClearAllTiles();
    }

    private List<Vector3Int> GetTilesInRange(Vector2Int origin, int range)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist <= range)
                {
                    tiles.Add(new Vector3Int(origin.x + dx, origin.y + dy, 0));
                }
            }
        }
        return tiles;
    }
}