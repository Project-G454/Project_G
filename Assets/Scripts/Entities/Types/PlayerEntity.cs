using Core.Managers.Deck;
using Entities.Factories;

namespace Entities.Categories {
    public class Player: Entity {
        public DeckManager deckManager;

        public Player(
            int id,
            EntityData data
        ): base(id, data) {
        }
    }
}

