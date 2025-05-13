using Core.Entities;
using Core.Interfaces;
using Entities;
using Entities.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Managers {
    public class MapManager : MonoBehaviour, IManager {
        public static MapManager Instance;
        private GridManager _gridManager;
        private BattleManager _battleManager;
        private EntityManager _entityManager;

        private Camera _mainCamera;

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
            _battleManager = BattleManager.Instance;
            _entityManager = EntityManager.Instance;
            _mainCamera = Camera.main;
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                MoveTo(mouseWorldPos);
            }
        }

        public void MoveTo(Vector2 targetPos) {
            Vector2 clickPosition = new Vector2(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y));
                Tile selectedTile = _gridManager.GetTileAtPosition(clickPosition);

                if (selectedTile != null) {
                    ClearAllHighlights(); // 清除 tile 高亮（不是綠色範圍）
                    selectedTile.SetHighlight(true);

                    int playerId = _battleManager.currentEntity.entityId;
                    GameObject player = _entityManager.GetEntityObject(playerId);
                    MoveHandler moveHandler = player.GetComponent<MoveHandler>();

                    Vector2Int playerPos = Vector2Int.RoundToInt(player.transform.position);
                    Vector2Int clickedPos = Vector2Int.RoundToInt(selectedTile.transform.position);

                    // 點到自己顯示範圍
                    if (playerPos == clickedPos) {
                        DistanceManager.Instance.ClearHighlights();
                        DistanceManager.Instance.ShowReachableTiles(playerPos);
                        return;
                    }

                    // 點其他格子：檢查範圍
                    if (DistanceManager.Instance.IsTileInRange(playerPos, clickedPos)) {
                        moveHandler.MoveToTile(selectedTile);
                        DistanceManager.Instance.ClearHighlights(); // ✅ 移動後清除綠格
                    } else {
                        DistanceManager.Instance.ShowOutOfRangeWarning();
                    }
                }
        }

        public void ClearAllHighlights() {
            for (int x = 0; x < _gridManager.width; x++) {
                for (int y = 0; y < _gridManager.height; y++) {
                    Tile tile = _gridManager.GetTileAtPosition(new Vector2(x, y));
                    if (tile != null) {
                        tile.SetHighlight(false);
                    }
                }
            }
            // 不清除範圍格（已在移動後處理）
        }
    }
}
