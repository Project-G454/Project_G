using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Energy Card", menuName = "Cards/Energy")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class EnergyCardData: CardData {
        public int step;
        public override CardTypes type => CardTypes.ENERGY;
    }
}
