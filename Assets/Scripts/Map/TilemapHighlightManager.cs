using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Core.Managers;

public class TilemapHighlightManager: MonoBehaviour {
    [Header("Highlight Settings")]
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase hoverHighlightTile;    // 滑鼠懸停的高亮 Tile
    [SerializeField] private TileBase forceHighlightTile;    // 強制高亮的 Tile

    private GridManager _gridManager;
    private Camera _camera;
    private Vector2 _lastHoverPosition = Vector2.one * -999; // 追蹤上次懸停位置
    private HashSet<Vector2> _forceHighlightPositions = new HashSet<Vector2>(); // 強制高亮的位置

    void Start() {
        _gridManager = GridManager.Instance;
        _camera = Camera.main;
    }

    void Update() {
        HandleMouseHover();
    }

    private void HandleMouseHover() {
        Vector3 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = _gridManager.WorldToGridPosition(worldPos);

        // 如果滑鼠位置改變了
        if (gridPos != _lastHoverPosition) {
            // 移除上次的懸停高亮（如果不是強制高亮）
            if (_lastHoverPosition != Vector2.one * -999 && !_forceHighlightPositions.Contains(_lastHoverPosition)) {
                RemoveHoverHighlight(_lastHoverPosition);
            }

            // 添加新的懸停高亮（如果是可行走的位置且不是強制高亮）
            var tileData = _gridManager.GetTileAtPosition(gridPos);
            if (tileData != null && tileData.Walkable && !_forceHighlightPositions.Contains(gridPos)) {
                SetHoverHighlight(gridPos);
            }

            _lastHoverPosition = gridPos;
        }
    }

    private void SetHoverHighlight(Vector2 gridPos) {
        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);
        highlightTilemap.SetTile(tilePos, hoverHighlightTile);
    }

    private void RemoveHoverHighlight(Vector2 gridPos) {
        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);
        highlightTilemap.SetTile(tilePos, null);
    }

    // 對應原本的 SetHighlight 方法
    public void SetHighlight(Vector2 gridPos, bool active, bool needWalkable = true) {
        var tileData = _gridManager.GetTileAtPosition(gridPos);

        // 檢查是否符合條件
        if (tileData == null || (needWalkable && !tileData.Walkable)) {
            return;
        }

        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);

        if (active) {
            // 添加強制高亮
            _forceHighlightPositions.Add(gridPos);
            highlightTilemap.SetTile(tilePos, forceHighlightTile);
        }
        else {
            // 移除強制高亮
            _forceHighlightPositions.Remove(gridPos);
            highlightTilemap.SetTile(tilePos, null);
        }
    }

    // 清除所有高亮
    public void ClearAllHighlights() {
        highlightTilemap.SetTilesBlock(
            new BoundsInt(0, 0, 0, _gridManager.width, _gridManager.height, 1),
            new TileBase[_gridManager.width * _gridManager.height]
        );
        _forceHighlightPositions.Clear();
    }

    // 批次設置高亮（用於路徑顯示等）
    public void SetMultipleHighlights(List<Vector2> positions, bool active) {
        foreach (var pos in positions) {
            SetHighlight(pos, active);
        }
    }
}
