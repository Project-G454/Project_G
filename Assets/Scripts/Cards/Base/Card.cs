using System.Collections.Generic;
using Cards.Data;
using Core.Loaders.Cards;
using Entities;
using UnityEngine;

namespace Cards {
    /// <summary>
    /// 卡片基礎類別
    /// </summary>
    public class Card {
        private readonly CardData _cardData;                                // 所有卡片類別的共同資料
        public int id { get => _cardData.id; }                              // 卡片 ID
        public string cardName { get => _cardData.cardName; }               // 卡片名稱 (標題)
        public string description { get => _cardData.description; }         // 卡片敘述
        public int[] desctiptionIds { get => _cardData.desctiptionIds; }    // 卡片敘述中特殊效果的描述 id
        public EntityClasses[] classes { get => _cardData.classes; }        // 可使用此卡片的職業
        public CardRarity rarity { get => _cardData.rarity; }               // 卡片稀有度
        public int cost { get; set; }                                       // 出牌時需要消耗的點數
        public CardTypes type { get; set; }                                 // 卡片類型 (攻擊牌、魔法牌 ... 等)
        public int range { get => _cardData.range; }                         // 卡片範圍，0 代表只能用於自己
        public CardAnimations partical { get => _cardData.partical; }        // 使用卡片時作用在目標上的動畫
        // public List<int> effectIds { get => _cardData.}

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

        public virtual void ApplyView(CardView view) {
            view.title.text = cardName;
            view.cost.text = cost.ToString();

            view.background.sprite = CardDataLoader.LoadBackground(id);
            view.frame.sprite = CardDataLoader.LoadFrame(type);
            view.titleBackground.sprite = CardDataLoader.LoadTitleBG(rarity);
        }
    }
}
