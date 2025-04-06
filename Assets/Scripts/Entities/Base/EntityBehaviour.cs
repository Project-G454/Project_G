using Core.Managers.Deck;
using Core.Managers.Energy;
using Entities.Categories;
using Entities.Factories;
using UnityEngine;

namespace Entities {
    class EntityBehaviour: MonoBehaviour {
        public Entity entity;

        public void Init(Entity entity) {
            this.entity = entity;

            InitializeEntityDeck(entity);
            entity.energyManager = gameObject.AddComponent<EnergyManager>();
        }

        private void InitializeEntityDeck(Entity entity) {
            entity.deckManager = gameObject.AddComponent<DeckManager>();
            var initDeck = EntityFactory.GetClassDeck(entity.entityClass);
            foreach (var cardId in initDeck)
                entity.deckManager.AddCardToDeck(cardId);
        }

        void OnMouseDown() {
            if (this.entity is Player player) {
                Debug.Log(string.Join(", ", player.deckManager.deck.GetAllCards()));
            }
            entity.TakeDamage(10);
        }
    }
}
