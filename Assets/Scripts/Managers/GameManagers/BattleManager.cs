using System;
using System.Collections;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Loaders.Cards;
using Core.Managers.Cards;
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
        public Entity currentEntity;
        private int _id;
        private int _entityCount;

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
            this._id = 0;
            this._entityCount = _entityManager.GetEntityList().Count;
        }

        private void Start() {
            Init();
            StartCoroutine(GameLoop());
        }

        private void InitManagers() {
            _cardManager = ManagerHelper.RequireManager(CardManager.Instance);
            _entityManager = ManagerHelper.RequireManager(EntityManager.Instance);
            _effectManager = ManagerHelper.RequireManager(EffectManager.Instance);
            _gridManager = ManagerHelper.RequireManager(GridManager.Instance);
            _mapManager = ManagerHelper.RequireManager(MapManager.Instance);
            _cameraManager = ManagerHelper.RequireManager(CameraManager.Instance);
            _descriptionManager = ManagerHelper.RequireManager(DescriptionManager.Instance);
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
                NextPlayer();

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
            _id = (_id % _entityCount) + 1;
            currentEntity = _entityManager.GetEntity(_id);
            GameObject entityObject = _entityManager.GetEntityObject(_id);
            _cameraManager.SnapCameraTo(entityObject);
            MoveHandler moveHandler = entityObject.GetComponent<MoveHandler>();
            moveHandler.step = 1;
            Debug.Log(currentEntity.entityId);
        }
    }
}
