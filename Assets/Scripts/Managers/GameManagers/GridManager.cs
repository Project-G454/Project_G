using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Core.Managers {
    public class GridManager: MonoBehaviour, IManager {
        public static GridManager Instance;
        [SerializeField] private int _width, _height;

        [Header("Tilemap Setting")]
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap decorationTilemap;
        [SerializeField] private Tilemap obstacleTilemap;
        [SerializeField] private TilemapHighlightManager _highlightManager;

        [Header("BackGround Setting")]
        [SerializeField] public Tilemap backgroundTilemap;

        [Header("Terrain Style System")]
        [SerializeField] private List<TerrainStyleData> availableStyles = new List<TerrainStyleData>();
        [SerializeField] private int currentStyleIndex = 0;

        [Header("Roguelike Random Setting")]
        [Tooltip("Avoid consecutive occurrences of the same style")]
        [SerializeField] private int avoidSameStyleCount = 2;

        [Header("Random Room")]
        [SerializeField] private int _iterations = 10;
        [SerializeField] private int _walkLength = 10;
        [SerializeField] private int _boundaryOffset = 1;

        [Header("Wall Optimization")]
        [SerializeField] private int _wallThickness = 2;

        public Transform map;
        private Transform _cam;

        private RuleTile currentFloorTile;
        private RuleTile currentWallTile;
        private RuleTile currentDecorationTile;
        private RuleTile currentObstacleTile;
        private RuleTile currentBackgroundTile;
        private float decorationProbability = 0.3f;
        private float obstacleProbability = 0.15f;

        private List<int> recentStyleIndices = new List<int>();
        private Dictionary<Vector2, bool> walkableData;
        private HashSet<Vector2> obstaclePositions;

        public int totalWidth => _width + 18;
        public int totalHeight => _height + 10;

        public int width => _width;
        public int height => _height;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Reset() { }

        public void Init() {
            this._cam = Camera.main.transform;

            if (availableStyles.Count > 0) {
                SelectRandomStyle();
            }
            else {
                Debug.LogWarning("No venue styles available!");
            }
        }

        void Start() {
            // 可以在這裡設置 Tilemap 的 Sorting Layer
        }

        public void SelectRandomStyle() {
            if (availableStyles.Count == 0) {
                Debug.LogWarning("No venue styles available!");
                return;
            }

            // 過濾可以隨機選擇的風格，建立索引列表
            List<int> availableIndices = new List<int>();
            List<float> weights = new List<float>();

            for (int i = 0; i < availableStyles.Count; i++) {
                if (availableStyles[i].canBeRandomlySelected) {
                    if (avoidSameStyleCount > 0 && recentStyleIndices.Contains(i)) {
                        continue;
                    }

                    availableIndices.Add(i);
                    weights.Add(availableStyles[i].selectionWeight);
                }
            }

            if (availableIndices.Count == 0) {
                for (int i = 0; i < availableStyles.Count; i++) {
                    if (availableStyles[i].canBeRandomlySelected) {
                        availableIndices.Add(i);
                        weights.Add(availableStyles[i].selectionWeight);
                    }
                }
            }

            if (availableIndices.Count == 0) {
                Debug.LogWarning("沒有可以隨機選擇的風格！");
                return;
            }

            int selectedIndex = SelectWeightedRandom(availableIndices, weights);

            UpdateRecentStyleHistory(selectedIndex);

            LoadTerrainStyle(selectedIndex);

            Debug.Log($"隨機選擇了風格: {availableStyles[selectedIndex].styleName}");
        }

        private int SelectWeightedRandom(List<int> indices, List<float> weights) {
            float totalWeight = 0f;
            for (int i = 0; i < weights.Count; i++) {
                totalWeight += weights[i];
            }

            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            for (int i = 0; i < weights.Count; i++) {
                currentWeight += weights[i];
                if (randomValue <= currentWeight) {
                    return indices[i];
                }
            }

            return indices[0];
        }

        private void UpdateRecentStyleHistory(int styleIndex) {
            recentStyleIndices.Add(styleIndex);

            // 保持歷史記錄在指定長度內
            while (recentStyleIndices.Count > avoidSameStyleCount) {
                recentStyleIndices.RemoveAt(0);
            }
        }

        public void LoadTerrainStyle(int styleIndex) {
            if (styleIndex < 0 || styleIndex >= availableStyles.Count) {
                Debug.LogError($"風格索引 {styleIndex} 超出範圍！");
                return;
            }

            currentStyleIndex = styleIndex;
            TerrainStyleData style = availableStyles[styleIndex];

            currentFloorTile = style.floorTile;
            currentWallTile = style.wallTile;
            currentDecorationTile = style.decorationTile;
            currentObstacleTile = style.obstacleTile;
            currentBackgroundTile = style.backgroundTile;

            Vector3 wallPos = wallTilemap.transform.localPosition;
            wallPos.y = style.wallYOffset;
            wallTilemap.transform.localPosition = wallPos;

            Vector3 obstaclePos = obstacleTilemap.transform.localPosition;
            obstaclePos.y = style.obstacleYOffset;
            obstacleTilemap.transform.localPosition = obstaclePos;

            decorationProbability = style.decorationProbability;
            obstacleProbability = style.obstacleProbability;

            Debug.Log($"已載入風格: {style.styleName}");
        }

        public string GetCurrentStyleName() {
            if (currentStyleIndex >= 0 && currentStyleIndex < availableStyles.Count) {
                return availableStyles[currentStyleIndex].styleName;
            }
            return "未知風格";
        }

        public string[] GetAllStyleNames() {
            return availableStyles.Select(style => style.styleName).ToArray();
        }

        public string[] GetRandomSelectableStyleNames() {
            return availableStyles
                .Where(style => style.canBeRandomlySelected)
                .Select(style => style.styleName)
                .ToArray();
        }

        // -------------------地圖生成邏輯-------------------
        public void GenerateGrid() {
            walkableData = new Dictionary<Vector2, bool>();

            GenerateBackground();
            Vector2Int roomCenter = new Vector2Int(_width / 2, _height / 2);
            HashSet<Vector2Int> floorPositions = GenerateIrregularRoom(roomCenter);
            CreateTilesFromFloorPositions(floorPositions);
            GenerateObstacles(floorPositions);

            _cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        }

        private void GenerateBackground() {
            if (backgroundTilemap == null || currentBackgroundTile == null) {
                Debug.LogWarning("背景 Tilemap 或當前背景 Tile 未設置");
                return;
            }

            // 清空背景層
            backgroundTilemap.SetTilesBlock(
                new BoundsInt(0, 0, 0, totalWidth, totalHeight, 1),
                new TileBase[totalWidth * totalHeight]
            );

            for (int x = -9; x < totalWidth - 9; x++) {
                for (int y = -5; y < totalHeight - 5; y++) {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    backgroundTilemap.SetTile(tilePos, currentBackgroundTile); // 使用當前風格的背景
                }
            }
        }

        private void CreateTilesFromFloorPositions(HashSet<Vector2Int> floorPositions) {
            // 清空所有 Tilemap
            floorTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            wallTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            if (decorationTilemap != null) {
                decorationTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            }

            foreach (var pos in floorPositions) {
                Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
                floorTilemap.SetTile(tilePos, currentFloorTile); // 使用當前風格的地板

                walkableData[new Vector2(pos.x, pos.y)] = true;
            }

            HashSet<Vector2Int> wallPositions = FindWallPositions(floorPositions, _wallThickness);

            foreach (var pos in wallPositions) {
                if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height) {
                    Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);

                    floorTilemap.SetTile(tilePos, currentFloorTile); // 使用當前風格的地板
                    wallTilemap.SetTile(tilePos, currentWallTile);   // 使用當前風格的牆壁

                    walkableData[new Vector2(pos.x, pos.y)] = false;
                }
            }

            GenerateWallDecorations(wallPositions);
        }

        private void GenerateObstacles(HashSet<Vector2Int> floorPositions) {
            if (currentObstacleTile == null || obstacleTilemap == null) {
                Debug.LogWarning("當前風格的障礙物 Tile 或 Tilemap 未設置");
                return;
            }

            if (floorPositions == null || floorPositions.Count == 0) {
                return;
            }

            if (walkableData == null) {
                return;
            }

            if (obstaclePositions == null) {
                obstaclePositions = new HashSet<Vector2>();
            }

            // 清空當前障礙物 Tilemap
            obstacleTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);

            List<Vector2> validObstaclePositions = new List<Vector2>();

            foreach (var floorPos in floorPositions) {
                Vector2 pos = new Vector2(floorPos.x, floorPos.y);

                if (IsValidObstaclePosition(pos)) {
                    validObstaclePositions.Add(pos);
                }
            }

            foreach (var pos in validObstaclePositions) {
                if (Random.value <= obstacleProbability) {
                    try {
                        Vector3Int tilePos = new Vector3Int((int)pos.x, (int)pos.y, 0);
                        obstacleTilemap.SetTile(tilePos, currentObstacleTile); // 使用當前風格的障礙物

                        walkableData[pos] = false;
                        obstaclePositions.Add(pos);
                    }
                    catch (System.Exception e) {
                        Debug.LogError($"生成障礙物時發生錯誤，位置: {pos}, 錯誤: {e.Message}");
                    }
                }
            }

            Debug.Log($"在 {obstacleTilemap.name} 上生成了 {obstaclePositions.Count} 個障礙物");
        }

        private void GenerateWallDecorations(HashSet<Vector2Int> wallPositions) {
            if (currentDecorationTile == null || decorationTilemap == null) {
                return;
            }

            foreach (var pos in wallPositions) {
                if (Random.value <= decorationProbability) {
                    Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
                    decorationTilemap.SetTile(tilePos, currentDecorationTile); // 使用當前風格的裝飾
                }
            }
        }

        // 障礙物操作方法 (更新為使用當前風格)
        public void AddObstacle(Vector2 pos) {
            if (GetTileWalkable(pos) && !HasObstacle(pos)) {
                Vector3Int tilePos = new Vector3Int((int)pos.x, (int)pos.y, 0);
                if (obstacleTilemap != null && currentObstacleTile != null) {
                    obstacleTilemap.SetTile(tilePos, currentObstacleTile);
                }
                if (obstaclePositions == null) {
                    obstaclePositions = new HashSet<Vector2>();
                }
                obstaclePositions.Add(pos);
                walkableData[pos] = false;
            }
        }

        public void RemoveObstacle(Vector2 pos) {
            if (obstaclePositions != null && obstaclePositions.Contains(pos)) {
                Vector3Int tilePos = new Vector3Int((int)pos.x, (int)pos.y, 0);
                if (obstacleTilemap != null) {
                    obstacleTilemap.SetTile(tilePos, null);
                }
                obstaclePositions.Remove(pos);
                walkableData[pos] = true;
            }
        }

        public bool HasObstacle(Vector2 pos) {
            return obstaclePositions != null && obstaclePositions.Contains(pos);
        }

        // 重新生成網格 (會自動隨機選擇新風格)
        public void RegenerateGrid() {
            SelectRandomStyle(); // 每次重新生成都隨機選擇風格
            ClearGrid();
            GenerateGrid();
        }

        // 保留你所有原有的方法...
        public List<Vector3> GetSpawnPositions(int count) {
            List<Vector3> spawnPositions = new List<Vector3>();

            if (walkableData == null || walkableData.Count == 0) {
                Debug.LogError("地圖尚未生成，無法獲取生成位置");
                return spawnPositions;
            }

            List<Vector2> edgePositions = GetEdgeWalkablePositions();

            if (edgePositions.Count < count) {
                Debug.LogWarning($"找不到足夠的生成位置，需要 {count} 個，僅有 {edgePositions.Count} 個");
                return ConvertToVector3(edgePositions);
            }

            float minDistance = 2.0f;
            float maxDistance = 5.0f;

            List<Vector2> selectedPositions = new List<Vector2>();
            selectedPositions.Add(GetAndRemoveRandomPosition(edgePositions));

            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts && selectedPositions.Count < count && edgePositions.Count > 0; attempt++) {
                List<Vector2> validPositions = FindValidPositions(edgePositions, selectedPositions, minDistance, maxDistance);

                if (validPositions.Count == 0) {
                    validPositions = FindPositionsWithMinDistance(edgePositions, selectedPositions, minDistance);
                }

                if (validPositions.Count > 0) {
                    int randomIndex = Random.Range(0, validPositions.Count);
                    Vector2 selectedPos = validPositions[randomIndex];
                    selectedPositions.Add(selectedPos);
                    edgePositions.Remove(selectedPos);
                }
                else {
                    break;
                }
            }

            if (selectedPositions.Count < count) {
                AddClosestPositions(ref selectedPositions, ref edgePositions, count);
            }

            return ConvertToVector3(selectedPositions);
        }

        private Vector2 GetAndRemoveRandomPosition(List<Vector2> positions) {
            int index = Random.Range(0, positions.Count);
            Vector2 position = positions[index];
            positions.RemoveAt(index);
            return position;
        }

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

        private void AddClosestPositions(ref List<Vector2> selectedPositions, ref List<Vector2> candidates, int targetCount) {
            if (candidates.Count == 0) {
                candidates = GetEdgeWalkablePositions();

                foreach (Vector2 selected in selectedPositions) {
                    candidates.Remove(selected);
                }
            }

            Vector2 referencePos = selectedPositions[0];

            candidates.Sort((a, b) => {
                float distA = Vector2.Distance(a, referencePos);
                float distB = Vector2.Distance(b, referencePos);
                return distA.CompareTo(distB);
            });

            while (selectedPositions.Count < targetCount && candidates.Count > 0) {
                selectedPositions.Add(candidates[0]);
                candidates.RemoveAt(0);
            }
        }

        private List<Vector3> ConvertToVector3(List<Vector2> positions) {
            List<Vector3> result = new List<Vector3>();
            foreach (Vector2 pos in positions) {
                result.Add(new Vector3(pos.x, pos.y, -1f));
            }
            return result;
        }

        private List<Vector2> GetEdgeWalkablePositions() {
            List<Vector2> edgePositions = new List<Vector2>();

            List<Vector2> directions = new List<Vector2> {
                new Vector2(0, 1), new Vector2(1, 0),
                new Vector2(0, -1), new Vector2(-1, 0)
            };

            foreach (var pair in walkableData) {
                Vector2 pos = pair.Key;
                bool isWalkable = pair.Value;

                if (!isWalkable) continue;

                bool isEdge = false;
                foreach (var dir in directions) {
                    Vector2 neighborPos = pos + dir;
                    bool neighborWalkable = GetTileWalkable(neighborPos);

                    if (!neighborWalkable) {
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
                    currentPosition = ChooseSmartStartPosition(floorPositions);
                }
            }

            return floorPositions;
        }

        private Vector2Int ChooseSmartStartPosition(HashSet<Vector2Int> floorPositions) {
            Vector2 centroid = CalculateCentroid(floorPositions);

            float randomValue = Random.value;

            if (randomValue < 0.5f) {
                return GetNearCentroidPosition(floorPositions, centroid);
            }
            else if (randomValue < 0.8f) {
                return GetMainBodyEdgePosition(floorPositions, centroid);
            }
            else {
                return floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }

        private Vector2 CalculateCentroid(HashSet<Vector2Int> positions) {
            if (positions.Count == 0) return Vector2.zero;

            float sumX = 0, sumY = 0;
            foreach (var pos in positions) {
                sumX += pos.x;
                sumY += pos.y;
            }

            return new Vector2(sumX / positions.Count, sumY / positions.Count);
        }

        private Vector2Int GetNearCentroidPosition(HashSet<Vector2Int> floorPositions, Vector2 centroid) {
            var nearPositions = floorPositions
                .OrderBy(pos => Vector2.Distance(pos, centroid))
                .Take(Mathf.Min(10, floorPositions.Count))
                .ToList();

            return nearPositions[Random.Range(0, nearPositions.Count)];
        }

        private Vector2Int GetMainBodyEdgePosition(HashSet<Vector2Int> floorPositions, Vector2 centroid) {
            var sortedByDistance = floorPositions
                .OrderBy(pos => Vector2.Distance(pos, centroid))
                .ToList();

            int mainBodyCount = Mathf.RoundToInt(sortedByDistance.Count * 0.75f);
            var mainBodyPositions = new HashSet<Vector2Int>(sortedByDistance.Take(mainBodyCount));

            var edgePositions = GetEdgePositions(mainBodyPositions);

            if (edgePositions.Count > 0) {
                return edgePositions[Random.Range(0, edgePositions.Count)];
            }
            else {
                return GetNearCentroidPosition(floorPositions, centroid);
            }
        }

        private List<Vector2Int> GetEdgePositions(HashSet<Vector2Int> floorPositions) {
            List<Vector2Int> edgePositions = new List<Vector2Int>();

            List<Vector2Int> directions = new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(-1, 0)
            };

            foreach (var pos in floorPositions) {
                bool isEdge = false;
                foreach (var dir in directions) {
                    var neighbor = pos + dir;
                    if (!floorPositions.Contains(neighbor)) {
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

        private HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength) {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPosition);
            var previousPosition = startPosition;

            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(-1, 0)
            };

            for (int i = 0; i < walkLength; i++) {
                var direction = directions[Random.Range(0, directions.Count)];
                var newPosition = previousPosition + direction;
                path.Add(newPosition);
                previousPosition = newPosition;
            }

            return path;
        }

        private bool IsValidObstaclePosition(Vector2 centerPos) {
            if (walkableData == null) {
                Debug.LogError("walkableData 未初始化");
                return false;
            }

            if (!GetTileWalkable(centerPos)) {
                return false;
            }

            List<Vector2> nineGridOffsets = new List<Vector2> {
                new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
                new Vector2(-1,  0), new Vector2(0,  0), new Vector2(1,  0),
                new Vector2(-1,  1), new Vector2(0,  1), new Vector2(1,  1)
            };

            foreach (var offset in nineGridOffsets) {
                Vector2 checkPos = centerPos + offset;

                if (!GetTileWalkable(checkPos)) {
                    if (walkableData.ContainsKey(checkPos) && !walkableData[checkPos]) {
                        return false;
                    }
                    if (!walkableData.ContainsKey(checkPos)) {
                        return false;
                    }
                }
            }

            return true;
        }

        private HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions, int thickness) {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

            HashSet<Vector2Int> boundaries = FindBoundaryPositions(floorPositions);

            foreach (var boundary in boundaries) {
                for (int layer = 1; layer <= thickness; layer++) {
                    var expandedPositions = ExpandPositionByLayer(boundary, layer, floorPositions);
                    wallPositions.UnionWith(expandedPositions);
                }
            }

            return wallPositions;
        }

        private HashSet<Vector2Int> FindBoundaryPositions(HashSet<Vector2Int> floorPositions) {
            HashSet<Vector2Int> boundaries = new HashSet<Vector2Int>();

            List<Vector2Int> directions = new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(-1, 0)
            };

            foreach (var pos in floorPositions) {
                bool isBoundary = false;
                foreach (var dir in directions) {
                    var neighbor = pos + dir;
                    if (!floorPositions.Contains(neighbor)) {
                        isBoundary = true;
                        break;
                    }
                }

                if (isBoundary) {
                    boundaries.Add(pos);
                }
            }

            return boundaries;
        }

        private HashSet<Vector2Int> ExpandPositionByLayer(Vector2Int center, int layer, HashSet<Vector2Int> floorPositions) {
            HashSet<Vector2Int> expanded = new HashSet<Vector2Int>();

            for (int x = -layer; x <= layer; x++) {
                for (int y = -layer; y <= layer; y++) {
                    if (Mathf.Abs(x) == layer || Mathf.Abs(y) == layer) {
                        var pos = center + new Vector2Int(x, y);

                        // 只有在不是地板的位置才加入牆壁
                        if (!floorPositions.Contains(pos)) {
                            expanded.Add(pos);
                        }
                    }
                }
            }

            return expanded;
        }

        public bool GetTileWalkable(Vector2 pos) {
            if (walkableData.TryGetValue(pos, out bool walkable)) {
                return walkable;
            }
            return false;
        }

        [System.Serializable]
        public class TileData {
            public Vector2 position;
            public bool walkable;

            public TileData(Vector2 pos, bool isWalkable) {
                position = pos;
                walkable = isWalkable;
            }

            public bool Walkable => walkable;
        }

        public TileData GetTileAtPosition(Vector2 pos) {
            if (walkableData.TryGetValue(pos, out bool walkable)) {
                return new TileData(pos, walkable);
            }
            return null;
        }

        public void SetTileWalkable(Vector2 pos, bool walkable) {
            walkableData[pos] = walkable;
        }

        public Vector2 WorldToGridPosition(Vector3 worldPosition) {
            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.y);
            return new Vector2(x, y);
        }

        public void ClearGrid() {
            if (walkableData != null) {
                walkableData.Clear();
            }

            // 清空所有 Tilemap
            if (floorTilemap != null) {
                floorTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            }
            if (wallTilemap != null) {
                wallTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            }
            if (decorationTilemap != null) {
                decorationTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            }
            if (obstacleTilemap != null) {
                obstacleTilemap.SetTilesBlock(new BoundsInt(0, 0, 0, _width, _height, 1), new TileBase[_width * _height]);
            }

            // 清空障礙物位置記錄
            if (obstaclePositions != null) {
                obstaclePositions.Clear();
            }
        }

        public void SetTileHighlight(Vector2 pos, bool active, bool needWalkable = true) {
            if (_highlightManager != null) {
                _highlightManager.SetHighlight(pos, active, needWalkable);
            }
        }

        public void ClearAllHighlights() {
            if (_highlightManager != null) {
                _highlightManager.ClearAllHighlights();
            }
        }
    }
}
