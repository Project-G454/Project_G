namespace Entities {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class PlayerData: EntityData {
        public int playerId = -1;
        public PlayerData(
            int playerId,
            int maxHealth,
            string entityName,
            EntityTypes type,
            EntityClasses entityClass
        ) : base(maxHealth, entityName, type, entityClass) {
            this.playerId = playerId;
        }
    }
}
