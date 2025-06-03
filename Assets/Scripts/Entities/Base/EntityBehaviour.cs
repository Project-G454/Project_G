using Core.Managers.Deck;
using Core.Managers.Energy;
using Core.Managers;
using Entities.Categories;
using Entities.Factories;
using UnityEngine;
using System.Collections.Generic;

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
            List<int> initDeck = new();
            if (entity is Player player) {
                initDeck = PlayerStateManager.Instance.GetPlayer(entity.entityId).deck;
            }
            else {
                initDeck = EntityFactory.GetClassDeck(entity.entityClass);
            }
            
            foreach (var cardId in initDeck)
                entity.deckManager.AddCardToDeck(cardId);
        }

        void OnMouseEnter() {
            HoverUIManager.Instance.Show(entity);
        }

        void OnMouseExit() {
            HoverUIManager.Instance.Hide();
        }
    }
}
