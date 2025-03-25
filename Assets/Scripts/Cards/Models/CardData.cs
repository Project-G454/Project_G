using Entities;

namespace Cards.Data {

    /// <summary>
    /// 所有卡片需要的基本資料
    /// </summary>
    public class CardData {
        public int id = 0;                                      // 卡片 ID
        public string cardName = "Unknown";                     // 卡片名稱 (標題)
        public string description = "";                         // 卡片敘述
        public EntityClasses[] classes = new EntityClasses[] {  // 可使用此卡片的職業
            EntityClasses.UNSET 
        };   
        public CardRarity rarity = CardRarity.UNSET;            // 卡片稀有度
        public int cost = 0;                                    // 出牌時需要消耗的點數
        public CardTypes type = CardTypes.UNSET;                // 卡片類型 (攻擊牌、魔法牌 ... 等)
    }
}
