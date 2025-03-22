using System.Collections.Generic;
using UnityEngine;
using Piles;

namespace Core.Managers.Deck {
    /// <summary>
    /// 管理玩家卡組邏輯：抽牌堆、手牌、棄牌堆皆為 Pile 類別。
    /// </summary>
    public class DeckManager: MonoBehaviour {
        public Pile playerDeck = new();
        public Pile draw = new();
        public Pile hand = new();
        public Pile discard = new();

        public int handSize = 5;
        
        public void InitializeDeck() {
            draw.Clear();
            hand.Clear();
            discard.Clear();
            draw.Add(this.playerDeck.GetAllCards());
            draw.Shuffle();
        }

        public void DrawCards(int amount) {
            for (int i = 0; i < amount; i++) {
                if (draw.Count == 0)
                    ReshuffleDiscardIntoDraw();

                var cardId = draw.Draw();
                if (cardId != null)
                    hand.Add(cardId.Value);
            }
        }

        public void DiscardHand() {
            discard.Add(hand.cardIds);
            hand.Clear();
        }

        private void ReshuffleDiscardIntoDraw() {
            draw.Add(discard.cardIds);
            discard.Clear();
            draw.Shuffle();
        }

        public void MoveCard(Pile source, Pile destination, int cardId) {
            if (source.Remove(cardId))
                destination.Add(cardId);
        }

        /// <summary>
        /// 玩家使用一張卡牌。
        /// </summary>
        public void Use(int cardId) {
            MoveCard(hand, discard, cardId);
        }

        /// <summary>
        /// 新增卡牌到玩家卡組。
        /// </summary>
        public void AddCardToPlayerDeck(int cardId) {
            playerDeck.Add(cardId);
        }

        /// <summary>
        /// 移除玩家卡組中的特定卡牌。
        /// </summary>
        public bool RemoveCardFromPlayerDeck(int cardId) {
            return playerDeck.Remove(cardId);
        }
    }
}
