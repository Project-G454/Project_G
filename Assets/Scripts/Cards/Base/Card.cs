using UnityEngine;

namespace Cards {
    /// <summary>
    /// Base class for all cards.
    /// </summary>
    public class Card {
        private readonly CardData _cardData;
        public int id { get => _cardData.id; }                        // ID of the card.
        public string cardName { get => _cardData.cardName; }         // The card's title or name.
        public string description { get => _cardData.description; }   // A description of the card's effect or purpose.
        public string[] classes { get => _cardData.classes; }         // The character classes that can use this card.
        public CardRarity rarity { get => _cardData.rarity; }         // The Rarity of the card (e.g. COMMON, EPIC).
        public int cost { get; set; }                                // The cost required to use the card.
        public CardTypes type { get; set; }                          // The type of the card (e.g. ATTACK, MAGIC).

        /// <summary>
        /// Constructor for the base Card class.
        /// </summary>
        /// <param name="data"><see cref="CardData"/></param>
        public Card(
            CardData cardData
        ) {
            _cardData = cardData;
            cost = cardData.cost;
            type = cardData.type;
        }

        // Defines how the card is used.
        public virtual void Use(int sourceId, int targetId) {
            Debug.Log($"Player_{sourceId} [Card_{this.id}] -> Player_{targetId}");
        }

        // Defines how the card is drop
        public virtual void Drop() {

        }
    }
}
