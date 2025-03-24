using Cards.Factories;
using Core.Entities;
using Core.Managers.Cards;
using Core.Managers.Deck;
using Entities;
using Entities.Categories;
using UnityEngine;

namespace Core.Managers {
    class BattleManager: MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        private EntityManager _entityManager;
        private CardManager _cardManager;
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
            
            StartTurn();
        }

        private void InitManagers() {
            _entityManager = EntityManager.Instance;
            _cardManager = CardManager.Instance;
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

            _entityManager.CreateEntity(data1, new Vector3(-150, 0, 0));
            _entityManager.CreateEntity(data2, new Vector3(0, 0, 0));
            _entityManager.CreateEntity(data3, new Vector3(150, 0, 0));
        }

        private void InitDecks() {
            foreach (Player player in EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER)) {
                player.deckManager.InitializeDeck();
            }
        }

        public void StartTurn() {
            _cardManager.StartTurn();
        }

        public void OnCardPlayed() {
            EndTurn();
        }

        public void EndTurn() {
            NextPlayer();
            StartTurn();
        }

        public void NextPlayer() {
            _id = (_id % _entityCount) + 1;
            currentEntity = _entityManager.GetEntity(_id);
            Debug.Log(currentEntity.entityId);
        }
    }
}
