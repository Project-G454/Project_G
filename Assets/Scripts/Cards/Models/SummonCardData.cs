using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Summon Card", menuName = "Cards/Summon")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class SummonCardData: CardData {
        public int summonAmount;
        public override CardTypes type => CardTypes.SUMMON;
    }
}
