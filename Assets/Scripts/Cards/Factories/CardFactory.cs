using Cards.Categories;
using Cards.Data;
using Entities;

namespace Cards.Factories {
    public static class CardFactory {
        /// <summary>
        /// 根據不同卡片類型，建立對應的子類別卡片實例。
        /// </summary>
        /// <remarks>
        /// 輸入資料類型應符合對應的卡片類型。
        /// </remarks>
        /// <param name="cardData">卡片資料。</param>
        /// <returns>根據卡片類型建立的 <see cref="Cards.Categories"/> 子類別卡片。</returns>
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

        /// <summary>
        /// 根據 id 回傳測試用的卡片資料。
        /// </summary>
        public static CardData GetFakeCardData(int id) {
            CardTypes type = (CardTypes)(id % 3 + 1);

            CardData cardData = type switch {
                CardTypes.ATTACK => new AttackCardData(),
                CardTypes.MAGIC => new MagicCardData(),
                CardTypes.MOVE => new MoveCardData(),
                _ => throw new System.NotImplementedException(),
            };

            cardData.id = id;
            cardData.cost = id % 9 + 1;
            cardData.cardName = "Card " + id.ToString();
            cardData.description = "Description " + id.ToString();
            cardData.classes = new EntityClasses[] { EntityClasses.UNSET };
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
