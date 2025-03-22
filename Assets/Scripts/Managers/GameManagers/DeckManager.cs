using System.Collections.Generic;
using UnityEngine;
using Piles;

namespace Core.Managers.Deck {
    /// <summary>
    /// 管理玩家卡組邏輯：抽牌堆、手牌、棄牌堆皆為 Pile 類別。
    /// </summary>
    public class DeckManager: MonoBehaviour {
        public Pile draw = new Pile();
        public Pile hand = new Pile();
        public Pile discard = new Pile();

        public int handSize = 5;

        public DeckManager() { }

        public DeckManager(List<int> initialDeck) {
            InitializeDeck(initialDeck);
        }

        public void InitializeDeck(List<int> startingCardIds) {
            draw.Add(startingCardIds);
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

        public void MoveCard(Pile source, Pile destination, IEnumerable<int> cardIds) {
            foreach (var cardId in cardIds) {
                if (source.Remove(cardId))
                    destination.Add(cardId);
            }
        }

        /// <summary>
        /// 玩家使用一張卡牌。
        /// </summary>
        public void Use(int cardId) {

        }
    }
}
