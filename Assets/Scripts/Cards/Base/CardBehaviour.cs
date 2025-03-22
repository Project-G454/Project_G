using UnityEngine;

namespace Cards {
    class CardBehaviour: MonoBehaviour {
        public Card card;
        public CardView cardView;

        public void Init(Card card) {
            this.card = card;
            this.cardView = this.GetComponent<CardView>();
            this.cardView.SetCardView(card);
        }
    }
}
