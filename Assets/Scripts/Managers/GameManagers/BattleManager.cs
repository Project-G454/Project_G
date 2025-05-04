using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Loaders.Cards;
using Core.Managers.Cards;
using Core.Managers.Dices;
using Dices;
using Entities;
using Entities.Categories;
using Entities.Handlers;
using UnityEngine;

namespace Core.Managers {
    public class BattleManager: MonoBehaviour, IManager {
        public static BattleManager Instance { get; private set; }
        private EntityManager _entityManager;
        private CardManager _cardManager;
        private EffectManager _effectManager;
        private GridManager _gridManager;
        private MapManager _mapManager;
        private CameraManager _cameraManager;
        private DescriptionManager _descriptionManager;
        private DiceManager _diceManager;
        private HoverUIManager _hoverUIManager;
        public Entity currentEntity;
        private int _turn;
        private int _round;
        private int _entityCount;
        private List<int> _orderedIds = new();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            InitManagers();
            InitMap();
            InitEntities();
            InitDeckAndEnergy();
            this._entityCount = _entityManager.GetEntityList().Count;
            _turn = 0;
            _round = 1;
            _orderedIds.Clear();
            foreach (Entity entity in _entityManager.GetEntityList()) {
                _orderedIds.Add(entity.entityId);
            }
        }

        private void Start() {
            Init();
            StartCoroutine(GameLoop());
        }

        private void InitManagers() {
            _cardManager = ManagerHelper.RequireManager(CardManager.Instance);
            _entityManager = ManagerHelper.RequireManager(EntityManager.Instance);
            _effectManager = ManagerHelper.RequireManager(EffectManager.Instance);
            _hoverUIManager = ManagerHelper.RequireManager(HoverUIManager.Instance);
            _gridManager = ManagerHelper.RequireManager(GridManager.Instance);
            _mapManager = ManagerHelper.RequireManager(MapManager.Instance);
            _cameraManager = ManagerHelper.RequireManager(CameraManager.Instance);
            _descriptionManager = ManagerHelper.RequireManager(DescriptionManager.Instance);
            _diceManager = ManagerHelper.RequireManager(DiceManager.Instance);
        }

        private void InitMap() {
            _gridManager.GenerateGrid();
        }

        private void InitEntities() {
            EntityData data1 = new EntityData(
                80,
                "Player1",
                EntityTypes.PLAYER,
                EntityClasses.WARRIOR
            );

            EntityData data2 = new EntityData(
                90,
                "Player2",
                EntityTypes.PLAYER,
                EntityClasses.RANGER
            );

            EntityData data3 = new EntityData(
                100,
                "Player3",
                EntityTypes.PLAYER,
                EntityClasses.ROGUE
            );

            _entityManager.CreateEntity(data1, new Vector3(0, 0, 0));
            _entityManager.CreateEntity(data2, new Vector3(0, 0, 0));
            _entityManager.CreateEntity(data3, new Vector3(0, 0, 0));
        }

        private void InitDeckAndEnergy() {
            foreach (Entity entity in EntityManager.Instance.GetEntityList()) {
                entity.deckManager.InitializeDeck();
                entity.energyManager.InitializeEnergy();
            }
        }

        public IEnumerator GameLoop() {
            while (true) {
                if (_IsRoundEnd()) {
                    Debug.Log("Dice Phase");
                    _round++;
                    yield return InitializeTurnOrder();
                }

                NextPlayer();
                if (currentEntity.IsDead()) continue;

                Debug.Log("Effect Phase (Before)");
                _effectManager.BeforeTurn();
                yield return new WaitUntil(() => _effectManager.isTurnFinished);

                Debug.Log("Card Phase");
                _cardManager.StartTurn();
                yield return new WaitUntil(() => _cardManager.isTurnFinished);

                Debug.Log("Effect Phase (After)");
                _effectManager.AfterTurn();
                yield return new WaitUntil(() => _effectManager.isTurnFinished);
            }
        }

        public void NextPlayer() {
            int idx = _turn % _entityCount;
            currentEntity = _entityManager.GetEntity(_orderedIds[idx]);
            
            GameObject entityObject = _entityManager.GetEntityObject(_orderedIds[idx]);
            _cameraManager.SnapCameraTo(entityObject);
            
            MoveHandler moveHandler = entityObject.GetComponent<MoveHandler>();
            moveHandler.step = 1;
            
            _turn++;
            Debug.Log($"Turn: Entity_{currentEntity.entityId}");
        }

        public IEnumerator InitializeTurnOrder() {
            Dictionary<int, int> points = new();
            foreach (int id in _orderedIds) {
                GameObject entityObj = _entityManager.GetEntityObject(id);
                _cameraManager.SnapCameraTo(entityObj);

                Entity entity = _entityManager.GetEntity(id);
                if (entity.IsDead()) {
                    points[id] = -1;
                    continue;
                }

                List<int> dicePoints = _diceManager.Roll(1, 6, 2);
                int point = dicePoints.Sum();
                yield return new WaitUntil(() => _diceManager.IsAllAnimationStopped());
                points[id] = point;

                string displayPoints = string.Join("+", dicePoints);
                Debug.Log($"Entity {id}: {displayPoints}={point}");
            }
            _orderedIds = points.OrderByDescending(e => e.Value)
                                .Select(e => e.Key)
                                .ToList();
            _diceManager.ResetDiceObjects();

            string order = "Entity_" + string.Join(", Entity_", _orderedIds);
            Debug.Log($"Order: {order}");
        }

        private bool _IsRoundEnd() {
            return (_turn % _entityCount) == 0;
        }
    }
}
