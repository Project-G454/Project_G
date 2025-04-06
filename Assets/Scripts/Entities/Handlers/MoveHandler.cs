using System;
using System.Collections.Generic;
using Core.Managers;
using Core.Managers.Energy;
using UnityEngine;

namespace Entities.Handlers {
    public class MoveHandler : MonoBehaviour {
        public float moveSpeed = 5f;
        public int step = 1;
        public Transform movePoint;
        private GridManager _gridManager;
        private MapManager _mapManager;
        private EnergyManager _energyManager;
        
        private Queue<Vector2> pathQueue = new Queue<Vector2>();
        private bool isMoving = false;
        private bool _endMoving = true;
        private Vector2 _nextPosition;

        void Init(){
            _gridManager = GridManager.Instance;
            _mapManager = MapManager.Instance;
            _energyManager = gameObject.GetComponent<EnergyManager>();
        }

        void Start() {
            Init();
            // movePoint.parent = null;
        }
        
        void Update() {
            if ((Vector2)transform.position == _nextPosition) {
                return;
            }

            _endMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, _nextPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, _nextPosition) <= 0.05f) {
                if (_gridManager != null) {
                    Vector2 gridPos = _gridManager.WorldToGridPosition(transform.position);
                    Tile currentTile = _gridManager.GetTileAtPosition(gridPos);
                    
                    if (currentTile != null) {
                        // Debug.Log($"角色位於格子: {currentTile.name}, 座標: ({gridPos.x}, {gridPos.y})");
                    }
                }
                
                // 如果還有路徑點，繼續移動
                if (pathQueue.Count > 0 && !isMoving) {
                    Vector2 nextCell = pathQueue.Dequeue();
                    _nextPosition = new Vector3(nextCell.x, nextCell.y, 0);
                    isMoving = true;
                }
                else {
                    isMoving = false;
                }
            }
            
            if (!_endMoving && (Vector2)transform.position == _nextPosition) {
                _mapManager.ClearAllHighlights();
                _endMoving = true;
            }
        }
        
        // 移動到指定格子（使用改進的曼哈頓算法）
        public void MoveToTile(Tile targetTile) {
            if (targetTile == null || !targetTile.Walkable)
                return;
                
            Vector2 targetPosition = targetTile.transform.position; //目標座標
            Vector2 currentPosition = transform.position; //目前座標
            
            // 清空當前路徑
            pathQueue.Clear();
            
            // 使用BFS尋找最佳路徑，遵循曼哈頓移動規則
            List<Vector2> path = FindPathBFS(
                _gridManager.WorldToGridPosition(currentPosition), 
                _gridManager.WorldToGridPosition(targetPosition)
            );
            
            // 將路徑添加到隊列（跳過第一個點，因為那是起點）
            for (int i = 1; i < path.Count; i++) {
                pathQueue.Enqueue(path[i]);
            }
            
            if (step == 0) {
                if (_energyManager.energy > 0) {
                    step += 1;
                    _energyManager.Remove(1);
                } else {
                    return;
                }
            }

            step -= 1;
            Debug.Log(string.Format("step: {0}, energy: {1}", step, _energyManager.energy));

            // 開始移動到第一個點
            if (pathQueue.Count > 0) {
                Vector2 nextCell = pathQueue.Dequeue();
                _nextPosition = new Vector3(nextCell.x, nextCell.y, 0);
                isMoving = true;
            }
        }
        
        private List<Vector2> FindPathBFS(Vector2 start, Vector2 goal) {
            if (start == goal)
                return new List<Vector2> { start };
                
            // BFS需要的隊列
            Queue<Vector2> frontier = new Queue<Vector2>();
            frontier.Enqueue(start);
            
            // 追蹤每個點的來源，用於重建路徑
            Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
            cameFrom[start] = start; // 起點的來源就是自己
            
            Vector2[] directions = new Vector2[] {
                new Vector2(1, 0),  // 右
                new Vector2(-1, 0), // 左
                new Vector2(0, 1),  // 上
                new Vector2(0, -1)  // 下
            };
            
            bool foundPath = false;
            
            // BFS loop
            while (frontier.Count > 0) {
                Vector2 current = frontier.Dequeue();
                
                if (current == goal) {
                    foundPath = true;
                    break;
                }
                
                // 檢查四個方向
                foreach (Vector2 direction in directions) {
                    Vector2 next = current + direction;
                    
                    // 如果這個點還沒被訪問過
                    if (!cameFrom.ContainsKey(next)) {
                        // 檢查這個點是否有效且可行走
                        Tile nextTile = _gridManager.GetTileAtPosition(next);
                        if (nextTile != null && nextTile.Walkable) {
                            frontier.Enqueue(next);
                            cameFrom[next] = current;
                        }
                    }
                }
            }
            
            if (!foundPath) // not found
                return new List<Vector2>();
                
            // rebuild path
            List<Vector2> path = new List<Vector2>();
            Vector2 currentStep = goal;
            
            while (currentStep != start) {
                path.Add(currentStep);
                currentStep = cameFrom[currentStep];
            }
            
            path.Add(start);
            path.Reverse(); // 反轉路徑，從起點到終點
            
            return path;
        }
    }
}
