using System.Collections;
using Core.Entities;
using Core.Managers.Cards;
using Entities;
using Entities.Categories;
using UnityEngine;

namespace Core.Managers {
    class BattleManager: MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        private EntityManager _entityManager;
        private CardManager _cardManager;
        private CardPositionManager _cardPositionManager;
        private EffectManager _effectManager;
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

        private void Start() {
            InitManagers();
            InitEntities();
            InitDecks();

            this._id = 1;
            this._entityCount = _entityManager.GetEntityList().Count;
            currentEntity = _entityManager.GetEntity(_id);

            StartCoroutine(GameLoop());
        }

        private void InitManagers() {
            _entityManager = EntityManager.Instance;
            _cardManager = CardManager.Instance;
            _cardPositionManager = CardPositionManager.Instance;
            _cardPositionManager.Init();
            _effectManager = EffectManager.Instance;
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
                EntityClasses.WIZARD
            );

            _entityManager.CreateEntity(data1, new Vector3(-100, 0, 0));
            _entityManager.CreateEntity(data2, new Vector3(0, 0, 0));
            _entityManager.CreateEntity(data3, new Vector3(100, 0, 0));
        }

        private void InitDecks() {
            foreach (Player player in EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER)) {
                player.deckManager.InitializeDeck();
            }
        }

        public IEnumerator GameLoop() {
            while (true) {
                Debug.Log("Effect Phase");
                _effectManager.StartTurn(_id);
                yield return new WaitUntil(() => _effectManager.isTurnFinished);

                Debug.Log("Card Phase");
                _cardManager.StartTurn();
                yield return new WaitUntil(() => _cardManager.isTurnFinished);

                NextPlayer();
            }
        }

        public void NextPlayer() {
            _id = (_id % _entityCount) + 1;
            currentEntity = _entityManager.GetEntity(_id);
            Debug.Log(currentEntity.entityId);
        }
    }
}
