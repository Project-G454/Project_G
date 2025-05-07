using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Managers {
    public class GridManager : MonoBehaviour, IManager {
        public static GridManager Instance;
        [SerializeField] private int _width, _height;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Tile _obstacleTilePrefab; // 可以為障礙物使用不同的Tile預製體
        [SerializeField] private Color floorColor, wallColor;

        [Header("Random Room")]
        [SerializeField] private bool _generateIrregularRoom = true;
        [SerializeField] private int _iterations = 10;
        [SerializeField] private int _walkLength = 10;
        [SerializeField] private int _boundaryOffset = 1;

        public Transform map;
        private Transform _cam;

        private Dictionary<Vector2, Tile> tiles;

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
            this._cam = Camera.main.transform;
        }

        void Start() {
            // GenerateGrid();
        }

        public void GenerateRectangularGrid() {
            tiles = new Dictionary<Vector2, Tile>();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var spawnedTile = Instantiate(_tilePrefab, new Vector2(x, y), Quaternion.identity, map);
                    spawnedTile.name = $"Tile {x} {y}";

                    spawnedTile.Init(new Vector2(x, y));

                    tiles[new Vector2(x, y)] = spawnedTile;
                }
            }

            _cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        }

        public void GenerateGrid() {
            tiles = new Dictionary<Vector2, Tile>();
            
            if (_generateIrregularRoom) {
                Vector2Int roomCenter = new Vector2Int(_width / 2, _height / 2);
                HashSet<Vector2Int> floorPositions = GenerateIrregularRoom(roomCenter); // 設定所有座標
                CreateTilesFromFloorPositions(floorPositions); // 視覺化
            } else {
                GenerateRectangularGrid();
            }

            _cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        }

        public List<Vector3> GetSpawnPositions(int count) {
            List<Vector3> spawnPositions = new List<Vector3>();
            
            if (tiles == null || tiles.Count == 0) {
                Debug.LogError("Map not generated yet, cannot get spawn positions");
                return spawnPositions;
            }

            List<Vector2> edgePositions = GetEdgeWalkablePositions();
            
            if (edgePositions.Count < count) {
                Debug.LogWarning($"Cannot find enough spawn positions, need {count}, but only have {edgePositions.Count}");
                foreach (var pos in edgePositions) {
                    spawnPositions.Add(new Vector3(pos.x, pos.y, 0));
                }
                return spawnPositions;
            }
            
            int randomStartIndex = Random.Range(0, edgePositions.Count);
            Vector2 centerPosition = edgePositions[randomStartIndex];
            List<Vector2> selectedPositions = new List<Vector2>();
            selectedPositions.Add(centerPosition);
            edgePositions.RemoveAt(randomStartIndex);
            
            int maxAttempts = 100;
            int attempts = 0;
            
            while (selectedPositions.Count < count && attempts < maxAttempts && edgePositions.Count > 0) {
                attempts++;
                
                Vector2 closestPosition = Vector2.zero;
                float closestDistance = float.MaxValue;
                int closestIndex = -1;
                
                for (int i = 0; i < edgePositions.Count; i++) {
                    Vector2 pos = edgePositions[i];
                    
                    float distance = Vector2.Distance(pos, centerPosition);
                    
                    if (distance <= 3.0f && distance < closestDistance) {
                        closestDistance = distance;
                        closestPosition = pos;
                        closestIndex = i;
                    }
                }
                
                if (closestIndex != -1) {
                    selectedPositions.Add(closestPosition);
                    edgePositions.RemoveAt(closestIndex);
                } else {
                    break;
                }
            }
            
            if (selectedPositions.Count < count) {
                Debug.LogWarning($"Cannot find enough nearby positions, will use more distant positions");
                
                edgePositions = GetEdgeWalkablePositions();
                
                foreach (var selected in selectedPositions) {
                    edgePositions.Remove(selected);
                }
                
                edgePositions.Sort((a, b) => {
                    float distA = Vector2.Distance(a, centerPosition);
                    float distB = Vector2.Distance(b, centerPosition);
                    return distA.CompareTo(distB);
                });
                
                while (selectedPositions.Count < count && edgePositions.Count > 0) {
                    selectedPositions.Add(edgePositions[0]);
                    edgePositions.RemoveAt(0);
                }
            }
            
            foreach (var pos in selectedPositions) {
                spawnPositions.Add(new Vector3(pos.x, pos.y, 0));
            }
            
            return spawnPositions;
        }

        private List<Vector2> GetAllWalkablePositions() {
            List<Vector2> walkablePositions = new List<Vector2>();
            
            foreach (var pair in tiles) {
                if (pair.Value.Walkable) {
                    walkablePositions.Add(pair.Key);
                }
            }
            
            return walkablePositions;
        }

        private List<Vector2> GetEdgeWalkablePositions() {
            List<Vector2> edgePositions = new List<Vector2>();
            
            List<Vector2> directions = new List<Vector2> {
                new Vector2(0, 1),  // 上
                new Vector2(1, 0),  // 右
                new Vector2(0, -1), // 下
                new Vector2(-1, 0)  // 左
            };
            
            foreach (var pair in tiles) {
                Vector2 pos = pair.Key;
                Tile tile = pair.Value;
                
                if (!tile.Walkable) continue;
                
                // 檢查是否為邊緣位置 (至少有一個相鄰位置是牆壁或不存在)
                bool isEdge = false;
                foreach (var dir in directions) {
                    Vector2 neighborPos = pos + dir;
                    Tile neighborTile = GetTileAtPosition(neighborPos);
                    
                    if (neighborTile == null || !neighborTile.Walkable) {
                        isEdge = true;
                        break;
                    }
                }
                
                if (isEdge) {
                    edgePositions.Add(pos);
                }
            }
            
            return edgePositions;
        }

        private HashSet<Vector2Int> GenerateIrregularRoom(Vector2Int center) {
            var floorPositions = RunRandomWalk(center);
            
            // 確保房間在網格邊界內
            floorPositions = new HashSet<Vector2Int>(floorPositions.Where(
                pos => pos.x >= _boundaryOffset && 
                       pos.x < width - _boundaryOffset && 
                       pos.y >= _boundaryOffset && 
                       pos.y < height - _boundaryOffset));
            
            return floorPositions;
        }

        private HashSet<Vector2Int> RunRandomWalk(Vector2Int startPosition) {
            var currentPosition = startPosition;
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
            floorPositions.Add(currentPosition);
            
            for (int i = 0; i < _iterations; i++) {
                var path = SimpleRandomWalk(currentPosition, _walkLength);
                floorPositions.UnionWith(path);
                
                if (floorPositions.Count > 0) {
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                }
            }
            
            return floorPositions;
        }

        private HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength) {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPosition);
            var previousPosition = startPosition;
            
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),  // 上
                new Vector2Int(1, 0),  // 右
                new Vector2Int(0, -1), // 下
                new Vector2Int(-1, 0)  // 左
            };
            
            for (int i = 0; i < walkLength; i++) {
                var direction = directions[Random.Range(0, directions.Count)];
                var newPosition = previousPosition + direction;
                path.Add(newPosition);
                previousPosition = newPosition;
            }
            
            return path;
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
        
        // 重新生成網格
        public void RegenerateGrid() {
            ClearGrid();
            GenerateGrid();
        }
    }
}
