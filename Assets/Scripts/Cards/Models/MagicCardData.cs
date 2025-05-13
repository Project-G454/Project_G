using System.Collections.Generic;
using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Magic Card", menuName = "Cards/Magic")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class MagicCardData: CardData {
        public List<int> effectIds;
        public override CardTypes type => CardTypes.MAGIC;
    }
}
