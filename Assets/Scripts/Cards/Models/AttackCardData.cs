using Unity.VisualScripting;
using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Attack Card", menuName = "Cards/Attack")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class AttackCardData: CardData {
        public int damage;
        public int effectId = -1;
        public int attackTimes = 1;
        public override CardTypes type => CardTypes.ATTACK; 
    }
}
