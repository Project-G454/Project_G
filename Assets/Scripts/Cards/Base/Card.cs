using UnityEngine;

namespace Cards {
    /// <summary>
    /// Base class for all cards.
    /// </summary>
    public class Card {
        private readonly CardData cardData;
        public int id { get => cardData.id; }                        // ID of the card.
        public string cardName { get => cardData.cardName; }         // The card's title or name.
        public string description { get => cardData.description; }   // A description of the card's effect or purpose.
        public string[] classes { get => cardData.classes; }         // The character classes that can use this card.
        public CardRarity rarity { get => cardData.rarity; }         // The Rarity of the card (e.g. COMMON, EPIC).
        public int cost { get; set; }                                // The cost required to use the card.
        public CardTypes type { get; set; }                          // The type of the card (e.g. ATTACK, MAGIC).

        /// <summary>
        /// Constructor for the base Card class.
        /// </summary>
        /// <param name="data"><see cref="CardData"/></param>
        public Card(
            CardData cardData
        ) {
            this.cardData = cardData;
            this.cost = cardData.cost;
            this.type = cardData.type;
        }

        // Defines how the card is used.
        public virtual void Use(int sourceId, int targetId) {
            Debug.Log($"Player ID-{sourceId} [Card ID-{this.id}] -> Player ID-{targetId}");
        }

        // Defines how the card is drop
        public virtual void Drop() {

        }
    }
}
