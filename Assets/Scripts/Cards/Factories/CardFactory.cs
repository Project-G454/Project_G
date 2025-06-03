using Cards.Categories;
using Cards.Data;

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
                case CardTypes.ENERGY:
                    card = new EnergyCard(cardData as EnergyCardData);
                    break;
                case CardTypes.Heal:
                    card = new HealCard(cardData as HealCardData);
                    break;
                default:
                    break;
            }
            return card;
        }
    }
}
