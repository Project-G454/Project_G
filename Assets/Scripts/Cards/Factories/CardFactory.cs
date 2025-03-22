using Cards.Categories;

namespace Cards.Factories {
    public static class CardFactory {
        public static Card MakeCard(CardData cardData) {
            Card card = new Card(cardData);
                switch (cardData.type) {
                    case CardTypes.ATTACK:
                        card = new AttackCard(cardData as AttackCardData);
                        break;
                    case CardTypes.MAGIC:
                        card = new MagicCard(cardData as MagicCardData);
                        break;
                    case CardTypes.MOVE:
                        card = new MoveCard(cardData as MoveCardData);
                        break;
                    default:
                        break;
            }
            return card;
        }

        public static CardData GetFakeCardData(int id) {
            CardTypes type = (CardTypes)(id % 3 + 1);

            CardData cardData = (type) switch {
                CardTypes.ATTACK => new AttackCardData(),
                CardTypes.MAGIC => new MagicCardData(),
                CardTypes.MOVE => new MoveCardData(),
                _ => new CardData()
            };

            cardData.id = id;
            cardData.cost = id % 9 + 1;
            cardData.cardName = "Card " + id.ToString();
            cardData.description = "Description " + id.ToString();
            cardData.type = (CardTypes)(id % 3 + 1);
            cardData.classes = new string[] { "All" };
            cardData.rarity = (CardRarity)(id % 5 + 1);

            switch (type) {
                case CardTypes.ATTACK:
                    ((AttackCardData)cardData).damage = 5;
                    break;
                case CardTypes.MAGIC:
                    ((MagicCardData)cardData).effectId = 1;
                    break;
                case CardTypes.MOVE:
                    ((MoveCardData)cardData).step = 10;
                    break;
                default:
                    break;
            }

            return cardData;
        }
    }
}
