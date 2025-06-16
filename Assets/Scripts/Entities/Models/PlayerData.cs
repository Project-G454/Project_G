namespace Entities {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class PlayerData: EntityData {
        public int playerId = -1;
        public PlayerData(
            int playerId,
            int maxHp,
            int currentHp,
            string entityName,
            EntityTypes type,
            EntityClasses entityClass
        ) : base(maxHp, currentHp, entityName, type, entityClass) {
            this.playerId = playerId;
        }
    }
}
