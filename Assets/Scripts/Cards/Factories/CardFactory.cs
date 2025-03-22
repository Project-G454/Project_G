using Cards.Categories;

namespace Cards.Factories {
    public static class CardFactory {
        public static Card MakeCard(CardData cardData) {
            Card card = new Card(cardData);
                switch (cardData.type) {
                    case CardTypes.ATTACK:
                        card = new AttackCard(cardData, 5);
                        break;
                    case CardTypes.MAGIC:
                        card = new MagicCard(cardData, 5);
                        break;
                    case CardTypes.MOVE:
                        card = new MoveCard(cardData, 5);
                        break;
            }
            return card;
        }
    }
}
