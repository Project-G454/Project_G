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
            // 創建地圖生成器
            _mapGenerator = new RandomWalkGenerator(_randomWalkData);
        }

        public void GenerateGrid() {
            tiles = new Dictionary<Vector2, Tile>();
            
            // 使用生成器生成地圖數據
            Vector2Int roomCenter = new Vector2Int(_width / 2, _height / 2);
            
            HashSet<Vector2Int> floorPositions = _mapGenerator.GenerateMap(_width, _height, roomCenter, _boundaryOffset);
                
            CreateTilesFromFloorPositions(floorPositions); // 視覺化地圖
            
            _cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        }

        private void CreateTilesFromFloorPositions(HashSet<Vector2Int> floorPositions) {
            // 創建在地板位置上的Tile
            foreach (var pos in floorPositions) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector2(pos.x, pos.y), Quaternion.identity, map);
                spawnedTile.name = $"Tile {pos.x} {pos.y}";
                
                spawnedTile.Init(new Vector2(pos.x, pos.y));

                SpriteRenderer spriteRenderer = spawnedTile.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    spriteRenderer.color = floorColor;
                }
                
                tiles[new Vector2(pos.x, pos.y)] = spawnedTile;
            }
            
            // 創建牆壁
            HashSet<Vector2Int> wallPositions = FindWallPositions(floorPositions);
            foreach (var pos in wallPositions) {
                // 確保在網格範圍內
                if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height) {
                    Tile tileToUse = _obstacleTilePrefab != null ? _obstacleTilePrefab : _tilePrefab;
                    var spawnedTile = Instantiate(tileToUse, new Vector2(pos.x, pos.y), Quaternion.identity, map);
                    spawnedTile.name = $"Wall {pos.x} {pos.y}";
                    
                    spawnedTile.Init(new Vector2(pos.x, pos.y));
                    spawnedTile.SetWalkable(false); // 將牆壁設為不可走
                    
                    SpriteRenderer spriteRenderer = spawnedTile.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null) {
                        spriteRenderer.color = wallColor;
                    }

                    tiles[new Vector2(pos.x, pos.y)] = spawnedTile;
                }
            }
        }

        private HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions) {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
            
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),  // 上
                new Vector2Int(1, 0),  // 右
                new Vector2Int(0, -1), // 下
                new Vector2Int(-1, 0)  // 左
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
