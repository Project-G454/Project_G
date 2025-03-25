using Cards.Data;
using Entities;
using UnityEngine;

namespace Cards {
    /// <summary>
    /// 卡片基礎類別
    /// </summary>
    public class Card {
        private readonly CardData _cardData;                          // 所有卡片類別的共同資料
        public int id { get => _cardData.id; }                        // 卡片 ID
        public string cardName { get => _cardData.cardName; }         // 卡片名稱 (標題)
        public string description { get => _cardData.description; }   // 卡片敘述
        public EntityClasses[] classes { get => _cardData.classes; }         // 可使用此卡片的職業
        public CardRarity rarity { get => _cardData.rarity; }         // 卡片稀有度
        public int cost { get; set; }                                 // 出牌時需要消耗的點數
        public CardTypes type { get; set; }                           // 卡片類型 (攻擊牌、魔法牌 ... 等)

        /// <param name="data">卡片資料 <see cref="CardData"/></param>
        public Card(
            CardData cardData
        ) {
            _cardData = cardData;
            cost = cardData.cost;
            type = cardData.type;
        }

        // 出牌動作
        public virtual void Use(int sourceId, int targetId) {
            Debug.Log($"Player_{sourceId} [Card_{this.id}] -> Player_{targetId}");
        }

        // 棄牌動作
        public virtual void Drop() {

        }
    }
}
