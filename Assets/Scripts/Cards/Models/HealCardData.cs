using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Heal Card", menuName = "Cards/Heal")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class HealCardData: CardData {
        public int healingAmount;
        public override CardTypes type => CardTypes.HEAL;
    }
}
