using System.Collections.Generic;
using Core.Interfaces;
using Core.Map.Generators;
using UnityEngine;

namespace Core.Managers {
    public class GridManager : MonoBehaviour, IManager {
        public static GridManager Instance;

        [SerializeField] private int _width, _height;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Tile _obstacleTilePrefab;
        [SerializeField] private Color floorColor, wallColor;

        [Header("Sprite Overlay")]
        [SerializeField] private Sprite floorSprite;
        [SerializeField] private Sprite wallSprite;

        [Header("Map Generation")]
        [SerializeField] private int _boundaryOffset = 1;
        [SerializeField] private RandomWalkGeneratorData _randomWalkData;

        public Transform map;
        private Transform _cam;
        private Dictionary<Vector2, Tile> tiles;
        private IMapGenerator _mapGenerator;

        public int width => _width;
        public int height => _height;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            _cam = Camera.main.transform;
            _mapGenerator = new RandomWalkGenerator(_randomWalkData);
        }

        public void GenerateGrid() {
            tiles = new Dictionary<Vector2, Tile>();
            Vector2Int roomCenter = new Vector2Int(_width / 2, _height / 2);
            HashSet<Vector2Int> floorPositions = _mapGenerator.GenerateMap(_width, _height, roomCenter, _boundaryOffset);
            CreateTilesFromFloorPositions(floorPositions);
            _cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        }

        private void CreateTilesFromFloorPositions(HashSet<Vector2Int> floorPositions) {
            foreach (var pos in floorPositions) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector2(pos.x, pos.y), Quaternion.identity, map);
                spawnedTile.name = $"Tile {pos.x} {pos.y}";
                spawnedTile.Init(new Vector2(pos.x, pos.y));

                SpriteRenderer sr = spawnedTile.GetComponent<SpriteRenderer>();
                if (sr != null) {
                    sr.sprite = floorSprite; // ✅ 改為拖入 sprite
                    sr.color = floorColor;   // ✅ 疊加顏色
                }

                tiles[new Vector2(pos.x, pos.y)] = spawnedTile;
            }

            HashSet<Vector2Int> wallPositions = FindWallPositions(floorPositions);
            foreach (var pos in wallPositions) {
                if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height) {
                    Tile tileToUse = _obstacleTilePrefab != null ? _obstacleTilePrefab : _tilePrefab;
                    var spawnedTile = Instantiate(tileToUse, new Vector2(pos.x, pos.y), Quaternion.identity, map);
                    spawnedTile.name = $"Wall {pos.x} {pos.y}";
                    spawnedTile.Init(new Vector2(pos.x, pos.y));
                    spawnedTile.SetWalkable(false);

                    SpriteRenderer sr = spawnedTile.GetComponent<SpriteRenderer>();
                    if (sr != null) {
                        sr.sprite = wallSprite; // ✅ 改為拖入 sprite
                        sr.color = wallColor;   // ✅ 疊加顏色
                    }

                    tiles[new Vector2(pos.x, pos.y)] = spawnedTile;
                }
            }
        }

        private HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions) {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
            List<Vector2Int> directions = new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(-1, 0)
            };

            foreach (var pos in floorPositions) {
                foreach (var dir in directions) {
                    var neighborPos = pos + dir;
                    if (!floorPositions.Contains(neighborPos)) {
                        wallPositions.Add(neighborPos);
                    }
                }
            }

            return wallPositions;
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

        public void ClearGrid() {
            if (tiles != null) {
                foreach (var tile in tiles.Values) {
                    if (tile != null) {
                        Destroy(tile.gameObject);
                    }
                }
                tiles.Clear();
            }
        }

        public void RegenerateGrid() {
            ClearGrid();
            GenerateGrid();
        }
    }
}
