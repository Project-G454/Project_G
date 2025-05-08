using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Managers {
    public class SpawnPositionManager : MonoBehaviour, IManager {
        public static SpawnPositionManager Instance;
        
        [Header("Spawn Settings")]
        [SerializeField] private float _minDistance = 2.0f;
        [SerializeField] private float _maxDistance = 5.0f;
        
        private GridManager _gridManager;
        
        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void Init() {
            _gridManager = GridManager.Instance;
        }
        
        public List<Vector3> GetSpawnPositions(int count) {
            List<Vector3> spawnPositions = new List<Vector3>();

            if (_gridManager == null) {
                Debug.LogError("GridManager未初始化");
                return spawnPositions;
            }

            List<Vector2> edgePositions = GetEdgeWalkablePositions();

            // 檢查是否有足夠的位置或者邊緣位置為空
            if (edgePositions == null || edgePositions.Count < count) {
                Debug.LogWarning($"找不到足夠的生成位置，需要 {count} 個，僅有 {edgePositions?.Count ?? 0} 個");
                return ConvertToVector3(edgePositions ?? new List<Vector2>());
            }
            
            float minDistance = 2.0f;
            float maxDistance = 5.0f;
            
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
                    break;
                }
            }
            
            // 如果位置不夠，添加最接近的位置
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
            if (candidates.Count == 0) {
                candidates = GetEdgeWalkablePositions();
                
                // 移除已選位置
                foreach (Vector2 selected in selectedPositions) {
                    candidates.Remove(selected);
                }
            }
            
            // 參考點為第一個選擇的位置
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
                result.Add(new Vector3(pos.x, pos.y, 0));
            }
            return result;
        }
        
        // 獲取邊緣可行走位置
        private List<Vector2> GetEdgeWalkablePositions() {
            List<Vector2> edgePositions = new List<Vector2>();
            
            List<Vector2> directions = new List<Vector2> {
                new Vector2(0, 1),  // 上
                new Vector2(1, 0),  // 右
                new Vector2(0, -1), // 下
                new Vector2(-1, 0)  // 左
            };
            
            for (int x = 0; x < _gridManager.width; x++) {
                for (int y = 0; y < _gridManager.height; y++) {
                    Vector2 pos = new Vector2(x, y);
                    Tile tile = _gridManager.GetTileAtPosition(pos);
                    
                    if (tile == null || !tile.Walkable) continue;
                    
                    // 檢查是否為邊緣位置 (至少有一個相鄰位置是牆壁或不存在)
                    bool isEdge = false;
                    foreach (var dir in directions) {
                        Vector2 neighborPos = pos + dir;
                        Tile neighborTile = _gridManager.GetTileAtPosition(neighborPos);
                        
                        if (neighborTile == null || !neighborTile.Walkable) {
                            isEdge = true;
                            break;
                        }
                    }
                    
                    if (isEdge) {
                        edgePositions.Add(pos);
                    }
                }
            }
            
            return edgePositions;
        }
    }
}
