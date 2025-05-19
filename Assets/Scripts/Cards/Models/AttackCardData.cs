using System.Collections.Generic;
using Effects;
using Effects.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Attack Card", menuName = "Cards/Attack")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class AttackCardData: CardData {
        public int damage;
        public List<Effect> effects;
        public int attackTimes = 1;
        public override CardTypes type => CardTypes.ATTACK; 
    }
}
