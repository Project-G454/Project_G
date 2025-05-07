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
            
            // 基本檢查
            if (tiles == null || tiles.Count == 0) {
                Debug.LogError("地圖尚未生成，無法獲取生成位置");
                return spawnPositions;
            }

            // 獲取所有可用的邊緣位置
            List<Vector2> edgePositions = GetEdgeWalkablePositions();
            
            // 檢查是否有足夠的位置
            if (edgePositions.Count < count) {
                Debug.LogWarning($"找不到足夠的生成位置，需要 {count} 個，僅有 {edgePositions.Count} 個");
                return ConvertToVector3(edgePositions);
            }
            
            // 設置距離參數
            float minDistance = 2.0f;  // 最小距離
            float maxDistance = 5.0f;  // 最大距離
            
            // 隨機選擇第一個位置
            List<Vector2> selectedPositions = new List<Vector2>();
            selectedPositions.Add(GetAndRemoveRandomPosition(edgePositions));
            
            // 尋找其餘位置
            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts && selectedPositions.Count < count && edgePositions.Count > 0; attempt++) {
                // 尋找符合條件的位置
                List<Vector2> validPositions = FindValidPositions(edgePositions, selectedPositions, minDistance, maxDistance);
                
                // 如果沒有完全符合的位置，僅考慮最小距離
                if (validPositions.Count == 0) {
                    validPositions = FindPositionsWithMinDistance(edgePositions, selectedPositions, minDistance);
                }
                
                // 如果找到有效位置，添加一個隨機位置
                if (validPositions.Count > 0) {
                    int randomIndex = Random.Range(0, validPositions.Count);
                    Vector2 selectedPos = validPositions[randomIndex];
                    selectedPositions.Add(selectedPos);
                    edgePositions.Remove(selectedPos);
                } else {
                    break; // 無法找到更多符合條件的位置
                }
            }
            
            // 如果位置不夠，添加最接近的位置
            if (selectedPositions.Count < count) {
                AddClosestPositions(ref selectedPositions, ref edgePositions, count);
            }
            
            // 轉換為 Vector3 並返回
            return ConvertToVector3(selectedPositions);
        }

        // 從列表中隨機獲取並移除一個位置
        private Vector2 GetAndRemoveRandomPosition(List<Vector2> positions) {
            int index = Random.Range(0, positions.Count);
            Vector2 position = positions[index];
            positions.RemoveAt(index);
            return position;
        }

        // 查找同時滿足最小和最大距離要求的位置
        private List<Vector2> FindValidPositions(List<Vector2> candidates, List<Vector2> existingPositions, float minDist, float maxDist) {
            List<Vector2> validPositions = new List<Vector2>();
            
            foreach (Vector2 candidate in candidates) {
                bool isValid = true;
                bool hasOneInRange = false;
                
                foreach (Vector2 existing in existingPositions) {
                    float distance = Vector2.Distance(candidate, existing);
                    
                    if (distance < minDist) {
                        isValid = false;
                        break;
                    }
                    
                    if (distance <= maxDist) {
                        hasOneInRange = true;
                    }
                }
                
                if (isValid && hasOneInRange) {
                    validPositions.Add(candidate);
                }
            }
            
            return validPositions;
        }

        // 僅查找滿足最小距離要求的位置
        private List<Vector2> FindPositionsWithMinDistance(List<Vector2> candidates, List<Vector2> existingPositions, float minDist) {
            List<Vector2> validPositions = new List<Vector2>();
            
            foreach (Vector2 candidate in candidates) {
                bool isFarEnough = true;
                
                foreach (Vector2 existing in existingPositions) {
                    if (Vector2.Distance(candidate, existing) < minDist) {
                        isFarEnough = false;
                        break;
                    }
                }
                
                if (isFarEnough) {
                    validPositions.Add(candidate);
                }
            }
            
            return validPositions;
        }

        // 添加最接近參考點的位置
        private void AddClosestPositions(ref List<Vector2> selectedPositions, ref List<Vector2> candidates, int targetCount) {
            // 重新獲取所有邊緣位置（如果需要）
            if (candidates.Count == 0) {
                candidates = GetEdgeWalkablePositions();
                
                // 移除已選位置
                foreach (Vector2 selected in selectedPositions) {
                    candidates.Remove(selected);
                }
            }
            
            // 參考點為第一個選擇的位置
            Vector2 referencePos = selectedPositions[0];
            
            // 根據距離排序
            candidates.Sort((a, b) => {
                float distA = Vector2.Distance(a, referencePos);
                float distB = Vector2.Distance(b, referencePos);
                return distA.CompareTo(distB);
            });
            
            // 添加最接近的位置
            while (selectedPositions.Count < targetCount && candidates.Count > 0) {
                selectedPositions.Add(candidates[0]);
                candidates.RemoveAt(0);
            }
        }

        // 將 Vector2 列表轉換為 Vector3 列表
        private List<Vector3> ConvertToVector3(List<Vector2> positions) {
            List<Vector3> result = new List<Vector3>();
            foreach (Vector2 pos in positions) {
                result.Add(new Vector3(pos.x, pos.y, 0));
            }
            return result;
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
