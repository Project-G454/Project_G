using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Functional Card", menuName = "Cards/Functional")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class FunctionalCardData: CardData {
        public int drawCount;
        public override CardTypes type => CardTypes.FUNCTIONAL;
    }
}
