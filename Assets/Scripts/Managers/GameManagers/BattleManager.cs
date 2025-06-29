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
using Agents;
using Agents.Handlers;
using Entities.Animations;
using Entities.Categories;
using Core.Game;

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
        private CameraController _cameraController;
        public Entity currentEntity;
        private int _turn;
        private int _round;
        private int _entityCount;
        private List<int> _orderedIds = new();
        private bool _initLock = false;
        private bool _bindingAgentLock = false;
        private bool _isBattleEnd = false;
        private bool _allEnemiesDead = false;
        private bool _allPlayersDead = false;
        private List<EntityData> _entityDatas;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _bossAudioSource;

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

        public void EntryWithEntityData(List<EntityData> e, bool isBossLevel = false) {
            _entityDatas = e;
            if (isBossLevel) _bossAudioSource.Play();
            else _audioSource.Play();
        }

        private void _InitCamera() {
            _cameraController = Camera.main.GetComponent<CameraController>();
            Vector3Int minPos = _gridManager.backgroundTilemap.cellBounds.min;
            Vector3Int maxPos = _gridManager.backgroundTilemap.cellBounds.max;
            _cameraController.minBounds = new Vector2(minPos.x, minPos.y);
            _cameraController.maxBounds = new Vector2(maxPos.x, maxPos.y);
        }

        public void Init() {
            _InitManagers();
            _InitMap();
            _InitEntities();
            _InitCamera();

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
            List<GameObject> enemies = _entityManager.GetEntitiesObjectByType(EntityTypes.ENEMY);
            foreach (GameObject enemy in enemies) {
                if (enemy.GetComponent<EntityAgent>() == null) {
                    enemy.AddComponent<EntityAgent>();
                }
            }

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
            var players = PlayerStateManager.Instance.GetAllPlayer();
            // List<EntityData> _entityDatas = new();
            foreach (GamePlayerState player in players) {
                _entityDatas.Add(new PlayerData(
                    player.playerId,
                    player.maxHp,
                    player.currentHp,
                    player.playerName,
                    EntityTypes.PLAYER,
                    player.entityClass
                ));
            }

            List<Vector3> spawnPositions = _gridManager.GetSpawnPositions(_entityDatas.Count);

            for (int i = 0; i < _entityDatas.Count; i++) {
                var entity = _entityManager.CreateEntity(_entityDatas[i], spawnPositions[i]);
                entity.OnDeath += CheckWinOrLose;
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
                    _turn = 0;
                    _round++;
                    currentEntity = null;
                    _SetEntitiesShadow();
                    // _globalUIManager.turnPanelUI.UpdateTurnOrder(_orderedIds, 100000);
                    yield return InitTurnOrder();
                }

                NextPlayer();
                if (currentEntity.IsDead()) continue;
                _SetEntitiesShadow();

                bool isStunned = currentEntity.IsStunned();

                Debug.Log("Effect Phase (Before)");
                _effectManager.BeforeTurn();
                yield return new WaitUntil(() => _effectManager.isTurnFinished);

                if (currentEntity.IsDead()) continue;

                if (!isStunned) {
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
            if (_allEnemiesDead) {
                if (EntityManager.Instance.GetEntitiesByType(EntityTypes.ENEMY).Any(e => e.entityClass == EntityClasses.Boss)) {
                    LoadSceneManager.Instance.LoadVictoryScene();
                }
                else {
                    LoadSceneManager.Instance.LoadBattleRewardsScene();
                }
            }
            else {
                LoadSceneManager.Instance.LoadGameOverScene();
            }
            yield break;
        }

        private void _SetEntitiesShadow() {
            foreach (var entity in _entityManager.GetEntityList()) {
                GameObject entityObj = _entityManager.GetEntityObject(entity.entityId);
                SPUM_Prefabs entityController = entityObj.GetComponentInChildren<SPUM_Prefabs>();
                Color color = entity.type switch {
                    EntityTypes.PLAYER => new Color(0, 0, 255, 0.5f),
                    EntityTypes.ENEMY => new Color(255, 0, 0, 0.5f),
                    _ => new Color(100, 100, 100, 0.5f)
                };

                if (
                    currentEntity != null &&
                    entity.entityId == currentEntity.entityId &&
                    currentEntity.type == EntityTypes.PLAYER
                ) color = new Color(255, 202, 0, 0.5f);

                entityController.SetShadowColor(color);
            }
        }

        public void NextPlayer() {
            DistanceManager.Instance.ClearHighlights();

            int idx = _turn % _entityCount;
            currentEntity = _entityManager.GetEntity(_orderedIds[idx]);
            _globalUIManager.turnPanelUI.UpdateTurnOrder(_orderedIds, idx);

            GameObject entityObject = _entityManager.GetEntityObject(_orderedIds[idx]);
            _cameraController.target = entityObject.transform;
            _cameraController.isFollowing = true;

            HoverUIManager.Instance.Show(currentEntity);

            MoveHandler moveHandler = entityObject.GetComponent<MoveHandler>();

            moveHandler.freestep = Math.Min(moveHandler.freestep + 1, 1);
            _globalUIManager.freestepUI.SetVisible(true);

            _globalUIManager.energyUI.Bind(currentEntity.energyManager);
            currentEntity.energyManager.RecoverEnergy();

            _turn++;
            Debug.Log($"--------- Turn: Entity_{currentEntity.entityId} ---------");
        }

        public void AddTurnOreder(int entityId) {
            _orderedIds.Add(entityId);
            this._entityCount = _entityManager.GetEntityList().Count;
            int idx = _turn % _entityCount;
            _globalUIManager.turnPanelUI.UpdateTurnOrder(_orderedIds, idx);
        }

        public IEnumerator InitTurnOrder() {
            Dictionary<int, int> points = new();
            foreach (int id in _orderedIds) {
                GameObject entityObj = _entityManager.GetEntityObject(id);
                _cameraController.target = entityObj.transform;
                _cameraController.isFollowing = true;

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
        
        private void CheckWinOrLose() {
            _allEnemiesDead = EntityManager.Instance
                .GetEntitiesByType(EntityTypes.ENEMY)
                .Where(p => p.entityClass != EntityClasses.Minion)
                .All(p => p.IsDead());

            _allPlayersDead = EntityManager.Instance
                .GetEntitiesByType(EntityTypes.PLAYER)
                .All(p => p.IsDead());

            if (_allEnemiesDead) {
                Debug.Log("🎉 Victory!");
                _cardManager.EndTurn();
                _isBattleEnd = true;
            }
            else if (_allPlayersDead) {
                Debug.Log("💀 Defeat...");
                _cardManager.EndTurn();
                _isBattleEnd = true;
            }
        }
    }
}
