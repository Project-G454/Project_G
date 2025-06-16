using System;
using System.Collections.Generic;
using Core.Managers;
using Core.Managers.Energy;
using UnityEngine;

namespace Entities.Handlers {
    public class MoveHandler: MonoBehaviour {
        public float moveSpeed = 5f;
        public int freestep = 0;
        public Transform movePoint;
        private GridManager _gridManager;
        private MapManager _mapManager;
        private GlobalUIManager _globalUIManager;
        private EnergyManager _energyManager;

        private Queue<Vector2> pathQueue = new Queue<Vector2>();
        public bool isMoving = false;
        public bool endMoving = true;
        private Vector2 _nextPosition;
        private Vector2 _currentGridPosition;
        private PlayerObj _SPUMScript;
        private Action onMoveEnd;

        void Init() {
            _gridManager = GridManager.Instance;
            _mapManager = MapManager.Instance;
            _globalUIManager = GlobalUIManager.Instance;
            _energyManager = gameObject.GetComponent<EnergyManager>();
            _SPUMScript = GetComponentInChildren<PlayerObj>();
        }

        void Start() {
            Init();
            _nextPosition = transform.position;
            _currentGridPosition = _gridManager.WorldToGridPosition(transform.position);

            // 初始化時將位置設為不可行走
            _gridManager.SetTileWalkable(_currentGridPosition, false);
        }

        void Update() {
            if ((Vector2)transform.position == _nextPosition) {
                return;
            }

            endMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, _nextPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _nextPosition) <= 0.05f) {
                // 更新當前網格位置
                Vector2 newGridPos = _gridManager.WorldToGridPosition(transform.position);

                // 只有在網格位置實際變化時才更新可行走狀態
                if (newGridPos != _currentGridPosition) {
                    _gridManager.SetTileWalkable(_currentGridPosition, true);

                    _currentGridPosition = newGridPos;

                    _gridManager.SetTileWalkable(_currentGridPosition, false);
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

            if (isMoving) _SPUMScript.SetState(PlayerState.MOVE);

            if (!endMoving && (Vector2)transform.position == _nextPosition) {
                // _mapManager.ClearAllHighlights();
                // Tile tile = _gridManager.GetTileAtPosition(_nextPosition);
                _gridManager.SetTileHighlight(_nextPosition, false, false);
                // tile.SetHighlight(false, false);
                _SPUMScript.SetState(PlayerState.IDLE);
                endMoving = true;
                onMoveEnd?.Invoke();
                onMoveEnd = null;
            }
        }

        // 移動到指定格子（使用改進的曼哈頓算法）
        public void MoveToPosition(Vector2 targetPosition, Action onComplete = null) {
            // 檢查目標位置是否可行走
            if (!_gridManager.GetTileWalkable(targetPosition))
                return;

            Vector2 currentPosition = transform.position; //目前座標
            this.onMoveEnd += onComplete;

            Vector2 _dirVec = targetPosition - (Vector2)transform.position;
            Vector2 _dirMVec = _dirVec.normalized;
            if (_dirMVec.x > 0) {
                Vector3 scale = transform.localScale;
                scale.x = Math.Abs(scale.x) * -1;
                transform.localScale = scale;
            }
            else if (_dirMVec.x < 0) {
                Vector3 scale = transform.localScale;
                scale.x = Math.Abs(scale.x); // 乘 -1 = 水平翻轉
                transform.localScale = scale;
            }

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

            // 開始移動到第一個點
            if (pathQueue.Count > 0) {
                // 判斷能量是否夠用於產生額外步數
                if (freestep == 0) {
                    if (_energyManager.energy > 0) {
                        _energyManager.Remove(1);
                    }
                    else {
                        return;
                    }
                }
                else {
                    Debug.Log(string.Format("step: {0}, energy: {1}", freestep, _energyManager.energy));
                    freestep -= 1;
                    if (freestep == 0) _globalUIManager.freestepUI.SetVisible(false);
                }

                Vector2 nextCell = pathQueue.Dequeue();
                _nextPosition = new Vector3(nextCell.x, nextCell.y, -1);
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
                        var nextTile = _gridManager.GetTileAtPosition(next);
                        // Tile nextTile = _gridManager.GetTileAtPosition(next);
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

        public bool CanReachPosition(Vector2 targetPosition) {
            if (!_gridManager.GetTileWalkable(targetPosition)) {
                return false;
            }

            Vector2 currentPosition = _gridManager.WorldToGridPosition(transform.position);
            Vector2 targetGridPosition = _gridManager.WorldToGridPosition(targetPosition);

            List<Vector2> path = FindPathBFS(currentPosition, targetGridPosition);

            return path.Count > 0;
        }
    }
}
