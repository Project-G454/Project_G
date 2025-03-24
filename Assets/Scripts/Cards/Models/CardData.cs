namespace Cards {

    /// <summary>
    /// Represents the data required to create a card.
    /// </summary>
    public class CardData {
        public int id = 0;                                  // ID of the card.
        public int cost = 0;                                // The cost required to use the card.
        public string cardName = "Unknown";                // The card's title or name.
        public string description = "";                     // A description of the card's effect or purpose.
        public CardTypes type = CardTypes.UNSET;            // The type of the card (e.g. ATTACK, MAGIC).
        public string[] classes = new string[] { "All" };   // The character classes that can use this card.
        public CardRarity rarity = CardRarity.UNSET;        // The Rarity of the card (e.g. COMMON, EPIC).
    }
}
