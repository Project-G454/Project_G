using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Core.Managers;

public class TilemapHighlightManager: MonoBehaviour {
    [Header("Highlight Settings")]
    [SerializeField] private Tilemap hoverHighlightTilemap;
    [SerializeField] private Tilemap forceHighlightTilemap;
    [SerializeField] private TileBase hoverHighlightTile;
    [SerializeField] private TileBase forceHighlightTile;

    private GridManager _gridManager;
    private Camera _camera;
    private Vector2 _lastHoverPosition = Vector2.one * -999;
    private HashSet<Vector2> _forceHighlightPositions = new HashSet<Vector2>();

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
            if (_lastHoverPosition != Vector2.one * -999) {
                RemoveHoverHighlight(_lastHoverPosition);
            }

            var tileData = _gridManager.GetTileAtPosition(gridPos);
            if (tileData != null && tileData.Walkable) {
                SetHoverHighlight(gridPos);
            }

            _lastHoverPosition = gridPos;
        }
    }

    private void SetHoverHighlight(Vector2 gridPos) {
        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);
        hoverHighlightTilemap.SetTile(tilePos, hoverHighlightTile);
    }

    private void RemoveHoverHighlight(Vector2 gridPos) {
        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);
        hoverHighlightTilemap.SetTile(tilePos, null);
    }

    public void SetHighlight(Vector2 gridPos, bool active, bool needWalkable = true) {
        var tileData = _gridManager.GetTileAtPosition(gridPos);

        if (tileData == null || (needWalkable && !tileData.Walkable)) {
            return;
        }

        Vector3Int tilePos = new Vector3Int((int)gridPos.x, (int)gridPos.y, 0);

        if (active) {
            _forceHighlightPositions.Add(gridPos);
            forceHighlightTilemap.SetTile(tilePos, forceHighlightTile);
        }
        else {
            _forceHighlightPositions.Remove(gridPos);
            forceHighlightTilemap.SetTile(tilePos, null);
        }
    }

    public void ClearAllHighlights() {
        hoverHighlightTilemap.SetTilesBlock(
            new BoundsInt(0, 0, 0, _gridManager.width, _gridManager.height, 1),
            new TileBase[_gridManager.width * _gridManager.height]
        );

        forceHighlightTilemap.SetTilesBlock(
            new BoundsInt(0, 0, 0, _gridManager.width, _gridManager.height, 1),
            new TileBase[_gridManager.width * _gridManager.height]
        );

        _forceHighlightPositions.Clear();
    }

    public void ClearForceHighlights() {
        forceHighlightTilemap.SetTilesBlock(
            new BoundsInt(0, 0, 0, _gridManager.width, _gridManager.height, 1),
            new TileBase[_gridManager.width * _gridManager.height]
        );
        _forceHighlightPositions.Clear();
    }
}
