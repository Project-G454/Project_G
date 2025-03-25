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
                Vector2 clickPosition = new Vector2(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
                
                Tile selectedTile = _gridManager.GetTileAtPosition(clickPosition);
                
                if (selectedTile != null) {
                    ClearAllHighlights();
                    
                    selectedTile.SetHighlight(true);

                    int playerId = _battleManager.currentEntity.entityId;
                    GameObject player = _entityManager.GetEntityObject(playerId);
                    MoveHandler moveHandler = player.GetComponent<MoveHandler>();
                    moveHandler.MoveToTile(selectedTile);
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
        }
    }
}
