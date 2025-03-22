using System.Collections.Generic;
using UnityEngine;

namespace Deck {
    public class DeckManager {
        public List<int> draw;
        public List<int> hand;
        public List<int> discard;

        public DeckManager() {
            draw = new();
            hand = new();
            discard = new();
        }

        public DeckManager(List<int> initialDeck) : this() {
            InitializeDeck(initialDeck);
        }

        public void InitializeDeck(List<int> initialDeck) {
            draw.AddRange(initialDeck);
            Shuffle();
        }

        public void DrawCards(int amount) {
            for (int i = 0; i < amount; i++) {
                var cardId = draw[0];
                draw.RemoveAt(0);
                hand.Add(cardId);

                if (draw.Count == 0)
                    ReshuffleDiscardIntoDraw();
            }
        }

        public void DiscardHand() {
            discard.AddRange(hand);
            hand.Clear();
        }

        public void ReshuffleDiscardIntoDraw() {
            draw.AddRange(discard);
            discard.Clear();
            Shuffle();
        }

        public void Use(int cardId) {
            hand.Remove(cardId);
            discard.Add(cardId);
        }

        private void Shuffle() {
            for (int i = 0; i < draw.Count; i++) {
                int rand = Random.Range(i, draw.Count);
                (draw[i], draw[rand]) = (draw[rand], draw[i]);
            }
        }
    }
}