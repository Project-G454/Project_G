using System.Collections.Generic;
using Effects;
using Effects.Data;
using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Magic Card", menuName = "Cards/Magic")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class MagicCardData: CardData {
        public List<Effect> effects;
        public override CardTypes type => CardTypes.MAGIC;
    }
}
