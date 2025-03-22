namespace Entities {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class EntityData {
        public int maxHealth = 0;
        public string entityName = "Unknown";
        public EntityTypes type = EntityTypes.UNSET;

        public EntityData(
            int maxHealth,
            string entityName,
            EntityTypes type
        ) {
            this.maxHealth = maxHealth;
            this.entityName = entityName;
            this.type = type;
        }
    }
}
