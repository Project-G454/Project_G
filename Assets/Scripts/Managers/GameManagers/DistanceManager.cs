using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers {
    public class DistanceManager: MonoBehaviour, IManager {
        public static DistanceManager Instance;

        [Header("Settings")]
        public int maxDistance = 5;

        [Header("References")]
        // 移除不需要的 GameObject 相關欄位
        // public GameObject highlightTilePrefab;
        // public Transform highlightParent;
        public Text warningText;

        // 移除不需要的偏移設定
        // [Header("Optional")]
        // public Vector3 highlightOffset = Vector3.zero;

        // 追蹤高亮的位置，用於清除
        private List<Vector2> currentHighlightPositions = new List<Vector2>();

        void IManager.Init() {
            warningText?.gameObject.SetActive(false);
        }

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowReachableTiles(Vector2Int origin) {
            Debug.Log("ShowReachableTiles");
            ClearHighlights();

            for (int dx = -maxDistance; dx <= maxDistance; dx++) {
                for (int dy = -maxDistance; dy <= maxDistance; dy++) {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist <= maxDistance) {
                        Vector2Int tilePos = origin + new Vector2Int(dx, dy);

                        // 檢查是否可行走
                        if (!GridManager.Instance.GetTileWalkable((Vector2)tilePos)) continue;

                        // 使用 Tilemap 高亮系統
                        Vector2 pos = (Vector2)tilePos;
                        GridManager.Instance.SetTileHighlight(pos, true, true); // needWalkable = true

                        // 記錄位置以便後續清除
                        currentHighlightPositions.Add(pos);
                    }
                }
            }
        }

        public void ClearHighlights() {
            // 清除所有記錄的高亮位置
            foreach (var pos in currentHighlightPositions) {
                GridManager.Instance.SetTileHighlight(pos, false);
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
