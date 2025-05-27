using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Managers.Cards;
using Core.Managers.Dices;
using Entities;
using Entities.Handlers;
using UnityEngine;
using Core.Handlers;

namespace Core.Managers {
    public class BattleManager: MonoBehaviour, IManager, IEntryManager {
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
        private DistanceManager _distanceManager;
        private GlobalUIManager _globalUIManager;
        public Entity currentEntity;
        private int _turn;
        private int _round;
        private int _entityCount;
        private List<int> _orderedIds = new();
        private bool _initLock = false;
        private bool _bindingAgentLock = false;
        private bool _isBattleEnd = false;

        private void Awake() {
            Instance = this;
        }

        public void Reset() {
            Instance = null;
        }

        public void ResetAll() {
            _cardManager.Reset();

            Reset();
        }

        public void Entry() {
            // StartBattle();
        }

        public void Init() {
            _InitManagers();
            _InitMap();
            _InitEntities();
            InitDeckAndEnergy();
            this._entityCount = _entityManager.GetEntityList().Count;
            _turn = 0;
            _round = 1;
            _orderedIds.Clear();
            foreach (Entity entity in _entityManager.GetEntityList()) {
                _orderedIds.Add(entity.entityId);
            }
            _initLock = true;
        }

        public void BindAgents() {
            // GameObject enemy1 = _entityManager.GetEntityObject(1);
            // enemy1.AddComponent<EntityAgent>();
            // GameObject enemy2 = _entityManager.GetEntityObject(2);
            // enemy2.AddComponent<EntityAgent>();
            // GameObject enemy3 = _entityManager.GetEntityObject(3);
            // enemy3.AddComponent<EntityAgent>();
            GameObject enemy4 = _entityManager.GetEntityObject(4);
            enemy4.AddComponent<EntityAgent>();
            _bindingAgentLock = true;
        }

        private void Start() {
            StartBattle();
        }

        public void StartBattle() {
            Init();
            BindAgents();
            StartCoroutine(GameLoop());
        }

        private void _InitManagers() {
            _cardManager = ManagerHelper.RequireManager(CardManager.Instance);
            _entityManager = ManagerHelper.RequireManager(EntityManager.Instance);
            _effectManager = ManagerHelper.RequireManager(EffectManager.Instance);
            _hoverUIManager = ManagerHelper.RequireManager(HoverUIManager.Instance);
            _gridManager = ManagerHelper.RequireManager(GridManager.Instance);
            _mapManager = ManagerHelper.RequireManager(MapManager.Instance);
            _cameraManager = ManagerHelper.RequireManager(CameraManager.Instance);
            _descriptionManager = ManagerHelper.RequireManager(DescriptionManager.Instance);
            _diceManager = ManagerHelper.RequireManager(DiceManager.Instance);
            _distanceManager = ManagerHelper.RequireManager(DistanceManager.Instance);
            _hoverUIManager = ManagerHelper.RequireManager(HoverUIManager.Instance);
            _globalUIManager = ManagerHelper.RequireManager(GlobalUIManager.Instance);
        }

        private void _InitMap() {
            _gridManager.GenerateGrid();
        }

        private void _InitEntities() {
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

            EntityData data4 = new EntityData(
                100,
                "Enemy1",
                EntityTypes.ENEMY,
                EntityClasses.WIZARD
            );

            List<Vector3> spawnPositions = _gridManager.GetSpawnPositions(4);

            _entityManager.CreateEntity(data1, spawnPositions[0]);
            _entityManager.CreateEntity(data2, spawnPositions[1]);
            _entityManager.CreateEntity(data3, spawnPositions[2]);
            _entityManager.CreateEntity(data4, spawnPositions[3]);
        }

        private void InitDeckAndEnergy() {
            foreach (Entity entity in EntityManager.Instance.GetEntityList()) {
                entity.deckManager.InitializeDeck();
                entity.energyManager.InitializeEnergy();
            }
        }

        public void UnlockAgent() {
            GameObject entityObj = _entityManager.GetEntityObject(currentEntity.entityId);
            var agent = entityObj.GetComponent<AgentStateHandler>();
            if (agent != null) agent.Unlock();
        }

        public IEnumerator GameLoop() {
            yield return new WaitUntil(() => _initLock && _bindingAgentLock);
            while (!_isBattleEnd) {
                if (_IsRoundEnd()) {
                    Debug.Log("Dice Phase");
                    _round++;
                    currentEntity = null;
                    // _globalUIManager.turnPanelUI.UpdateTurnOrder(_orderedIds, 100000);
                    // yield return InitTurnOrder();
                }

                NextPlayer();
                if (currentEntity.IsDead()) continue;

                Debug.Log("Effect Phase (Before)");
                _effectManager.BeforeTurn();
                yield return new WaitUntil(() => _effectManager.isTurnFinished);

                if (currentEntity.IsDead()) continue;

                if (!currentEntity.IsStunned()) {
                    Debug.Log("Card Phase");
                    UnlockAgent();
                    _cardManager.StartTurn();
                    yield return new WaitUntil(() => _cardManager.isTurnFinished);
                }

                Debug.Log("Effect Phase (After)");
                _effectManager.AfterTurn();
                yield return new WaitUntil(() => _effectManager.isTurnFinished);

                _globalUIManager.energyUI.UnBind(currentEntity.energyManager);
            }

            ResetAll();
            SceneTransitionHelper.LoadWorldMapScene();
            yield break;
        }

        public void NextPlayer() {
            DistanceManager.Instance.ClearHighlights();
            
            int idx = _turn % _entityCount;
            currentEntity = _entityManager.GetEntity(_orderedIds[idx]);
            _globalUIManager.turnPanelUI.UpdateTurnOrder(_orderedIds, idx);
            
            GameObject entityObject = _entityManager.GetEntityObject(_orderedIds[idx]);
            _cameraManager.SnapCameraTo(entityObject);
            
            HoverUIManager.Instance.Show(currentEntity);

            MoveHandler moveHandler = entityObject.GetComponent<MoveHandler>();

            moveHandler.freestep = Math.Min(moveHandler.freestep + 1, 1);
            _globalUIManager.freestepUI.SetVisible(true);

            _globalUIManager.energyUI.Bind(currentEntity.energyManager);
            currentEntity.energyManager.RecoverEnergy();
            Debug.Log(currentEntity.entityId);
            
            _turn++;
            Debug.Log($"Turn: Entity_{currentEntity.entityId}");
        }

        public IEnumerator InitTurnOrder() {
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
                points[id] = point;

                string displayPoints = string.Join("+", dicePoints);
                Debug.Log($"Entity {id}: {displayPoints}={point}");
                
                yield return new WaitUntil(() => _diceManager.IsAllAnimationStopped());
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
