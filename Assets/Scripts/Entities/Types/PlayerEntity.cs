using Core.Managers.Deck;
using Core.Managers.Energy;
using Entities.Factories;

namespace Entities.Categories {
    public class Player: Entity {
        public Player(
            int id,
            EntityData data
        ): base(id, data) {
        }
    }
}

