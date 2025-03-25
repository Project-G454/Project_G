using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;

    private Dictionary<Vector2, Tile> tiles;

    public int width => _width;
    public int height => _height;

    void Start() {
        GenerateGrid();

        // SetTileWalkable(new Vector2(1, 1), false); //障礙test
        // SetTileWalkable(new Vector2(2, 2), false);
        // SetTileWalkable(new Vector2(3, 3), false);
        // SetTileWalkable(new Vector2(4, 4), false);
        // SetTileWalkable(new Vector2(5, 5), false);
    }

    void GenerateGrid() {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset, new Vector2(x, y));

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if (tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }
        return null;
    }

    public void SetTileWalkable(Vector2 pos, bool walkable) {
        var tile = GetTileAtPosition(pos);
        if (tile != null) {
            tile.SetWalkable(walkable);
        }
    }

    public Vector2 WorldToGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.y);
        return new Vector2(x, y);
    }
}