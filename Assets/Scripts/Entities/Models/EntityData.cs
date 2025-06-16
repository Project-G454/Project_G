namespace Entities {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class EntityData {
        public int maxHp = 0;
        public int currentHp = 0;
        public string entityName = "Unknown";
        public EntityTypes type = EntityTypes.UNSET;
        public EntityClasses entityClass = EntityClasses.UNSET;

        public EntityData(
            int maxHp,
            int currentHp,
            string entityName,
            EntityTypes type,
            EntityClasses entityClass
        ) {
            this.maxHp = maxHp;
            this.currentHp = currentHp;
            this.entityName = entityName;
            this.type = type;
            this.entityClass = entityClass;
        }
    }
}
