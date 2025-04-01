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
            
            if (this.entity is Player player)
            {
                InitializePlayerDeck(player);
                player.energyManager = gameObject.AddComponent<EnergyManager>();
            }
        }

        private void InitializePlayerDeck(Player player)
        {
            player.deckManager = gameObject.AddComponent<DeckManager>();
            var initDeck = EntityFactory.GetClassDeck(entity.entityClass);
            foreach (var cardId in initDeck)
                player.deckManager.AddCardToPlayerDeck(cardId);
        }

        void OnMouseDown() {
            if (this.entity is Player player) {
                Debug.Log(string.Join(", ", player.deckManager.playerDeck.GetAllCards())); 
            }
            entity.TakeDamage(10);
        }
    }
}
