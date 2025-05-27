using Core.Entities;
using Core.Interfaces;
using Entities;
using Entities.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Managers {
    public class MapManager: MonoBehaviour, IManager {
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
            var selectedTileData = _gridManager.GetTileAtPosition(clickPosition);

            if (selectedTileData != null) {
                ClearAllHighlights(); // 清除 tile 高亮（不是綠色範圍）
                _gridManager.SetTileHighlight(clickPosition, true); // 設置高亮

                int playerId = _battleManager.currentEntity.entityId;
                GameObject player = _entityManager.GetEntityObject(playerId);
                MoveHandler moveHandler = player.GetComponent<MoveHandler>();

                Vector2Int playerPos = Vector2Int.RoundToInt(player.transform.position);
                Vector2Int clickedPos = Vector2Int.RoundToInt(clickPosition); // 直接使用 clickPosition

                // 點到自己顯示範圍
                if (playerPos == clickedPos) {
                    DistanceManager.Instance.ClearHighlights();
                    DistanceManager.Instance.ShowReachableTiles(playerPos);
                    return;
                }

                if (DistanceManager.Instance.IsTileInRange(playerPos, clickedPos)) {
                    moveHandler.MoveToPosition(clickPosition);
                    Entity entity = _entityManager.GetEntity(playerId);

                    // 簡化版本 - 直接指派
                    entity.position = clickPosition;

                    Debug.Log($"Entity_{entity.entityId} Move to {entity.position}");
                    DistanceManager.Instance.ClearHighlights();
                }
                else {
                    DistanceManager.Instance.ShowOutOfRangeWarning();
                }
            }
        }

        public void ClearAllHighlights() {
            // 使用 GridManager 的統一高亮清除方法
            _gridManager.ClearAllHighlights();

            // 如果需要更細緻的控制，可以用這種方式：
            /*
            for (int x = 0; x < _gridManager.width; x++) {
                for (int y = 0; y < _gridManager.height; y++) {
                    _gridManager.SetTileHighlight(new Vector2(x, y), false);
                }
            }
            */
        }
    }
}
