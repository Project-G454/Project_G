using Cards.Interactions;
using UnityEngine;

namespace Cards {
    /// <summary>
    /// Base class for all cards.
    /// </summary>
    public class Card
    {
        public int id;              // ID of the card.
        public int cost;            // The cost required to use the card.
        public string name;         // The card's title or name.
        public string description;  // A description of the card's effect or purpose.
        public CardTypes type;      // The type of the card (e.g. ATTACK, MAGIC).
        public string[] classes;    // The character classes that can use this card.
        public CardRarity rarity;   // The Rarity of the card (e.g. COMMON, EPIC).
        

        /// <summary>
        /// Constructor for the base Card class.
        /// </summary>
        /// <param name="id">The unique identifier for the card.</param>
        /// <param name="name">The title or name of the card.</param>
        /// <param name="description">A brief explanation of the card's effect.</param>
        /// <param name="type">The type of the card (e.g., general, magic, move).</param>
        /// <param name="cost">The cost required to use the card.</param>
        /// <param name="classes">The character classes that can use this card.</param>
        public Card(
            CardData data
        ) {
            this.id = data.id;
            this.name = data.name;
            this.description = data.description;
            this.type = data.type;
            this.cost = data.cost;
            this.classes = data.classes;
            this.rarity = data.rarity;
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
