using UnityEngine;

namespace Cards.Data {
    [CreateAssetMenu(fileName = "New Move Card", menuName = "Cards/Move")]
    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class MoveCardData: CardData {
        public int step;
        public override CardTypes type => CardTypes.MOVE;
    }
}
