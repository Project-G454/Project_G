using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers {
    public class DistanceManager: MonoBehaviour, IManager {
        public static DistanceManager Instance;

        [Header("Settings")]
        public int maxDistance = 5;

        public Text warningText;

        // 追蹤高亮的位置，用於清除
        private List<Vector2> currentHighlightPositions = new List<Vector2>();

        void IManager.Init() {
            warningText?.gameObject.SetActive(false);
        }

        void IManager.Reset() {
            // 清除所有高亮
            ClearHighlights();

            // 隱藏警告文字
            warningText?.gameObject.SetActive(false);

            // 取消所有延遲調用（如果有正在等待的 HideWarning）
            CancelInvoke();
        }

        void Awake() {
            Instance = this;
        }

        public void ShowReachableTiles(Vector2Int origin, int range=-1, bool needWalkable=true) {
            if (range == -1) range = maxDistance;
            ClearHighlights();
            if (range == 0) {
                GridManager.Instance.SetTileHighlight(origin, true, false);
                currentHighlightPositions.Add(origin);
                return;
            }

            for (int dx = -range; dx <= range; dx++) {
                for (int dy = -range; dy <= range; dy++) {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist <= range) {
                        Vector2Int tilePos = origin + new Vector2Int(dx, dy);

                        // 檢查是否可行走
                        if (needWalkable && !GridManager.Instance.GetTileWalkable((Vector2)tilePos)) continue;

                        // 使用 Tilemap 高亮系統
                        Vector2 pos = (Vector2)tilePos;
                        GridManager.Instance.SetTileHighlight(pos, true, needWalkable); // needWalkable = true

                        // 記錄位置以便後續清除
                        currentHighlightPositions.Add(pos);
                    }
                }
            }
        }

        public void ClearHighlights() {
            // 清除所有記錄的高亮位置
            foreach (var pos in currentHighlightPositions) {
                GridManager.Instance.SetTileHighlight(pos, false, false);
            }
            currentHighlightPositions.Clear();
        }

        public bool IsTileInRange(Vector2Int origin, Vector2Int target) {
            int distance = Mathf.Abs(origin.x - target.x) + Mathf.Abs(origin.y - target.y);
            return distance <= maxDistance;
        }

        public void ShowOutOfRangeWarning() {
            if (warningText != null) {
                warningText.text = "超出可移動範圍!";
                warningText.color = Color.red;
                warningText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideWarning));
                Invoke(nameof(HideWarning), 2f);
            }
        }

        private void HideWarning() {
            warningText?.gameObject.SetActive(false);
        }
    }
}
