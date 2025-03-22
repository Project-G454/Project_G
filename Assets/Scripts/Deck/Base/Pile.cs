using System.Collections.Generic;
using UnityEngine;

namespace Piles {
    /// <summary>
    /// 表示一個基本卡牌堆疊（如抽牌、手牌、棄牌）。
    /// </summary>
    public class Pile {
        public List<int> cardIds = new List<int>();

        public void Add(int cardId) {
            cardIds.Add(cardId);
        }

        public void Add(IEnumerable<int> cardIdList) {
            cardIds.AddRange(cardIdList);
        }

        public bool Remove(int cardId) {
            return cardIds.Remove(cardId);
        }

        public int? Draw() {
            if (cardIds.Count == 0) return null;
            var card = cardIds[0];
            cardIds.RemoveAt(0);
            return card;
        }

        public void Clear() => cardIds.Clear();

        public void Shuffle() {
            for (int i = 0; i < cardIds.Count; i++) {
                int rand = Random.Range(i, cardIds.Count);
                (cardIds[i], cardIds[rand]) = (cardIds[rand], cardIds[i]);
            }
        }

        public int Count => cardIds.Count;

        public List<int> GetAllCards() => new List<int>(cardIds);
    }
}
