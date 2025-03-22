using UnityEngine;

namespace Entities {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class EntityData {
        public int health = 0;
        public int maxHealth = 0;
        public string entityName = "Unknown";
        public EntityTypes type = EntityTypes.UNSET;

        public EntityData(
            int health,
            int maxHealth,
            string entityName,
            EntityTypes type
        ) {
            this.health = health;
            this.maxHealth = maxHealth;
            this.entityName = entityName;
            this.type = type;
        }
    }
}
