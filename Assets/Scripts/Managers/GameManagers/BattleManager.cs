using Core.Entities;
using Core.Managers.Cards;
using Entities;
using UnityEngine;

namespace Core.Managers {
    class BattleManager: MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        public EntityManager entityManager;
        public CardManager cardManager;
        public Entity currentEntity;
        private int id;
        private int entityCount;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            this.id = 1;
            this.entityCount = entityManager.GetEntityList().Count;
            currentEntity = entityManager.GetEntity(id);
            StartTurn();
        }

        public void StartTurn() {
            cardManager.StartTurn();
        }

        public void OnCardPlayed() {
            EndTurn();
        }

        public void EndTurn() {
            NextPlayer();
            StartTurn();
        }

        public void NextPlayer() {
            id = (id % entityCount) + 1;
            currentEntity = entityManager.GetEntity(id);
            Debug.Log(currentEntity.entityId);
        }
    }
}
