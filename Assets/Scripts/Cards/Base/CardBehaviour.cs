using Core.Managers.Cards;
using UnityEngine;

namespace Cards {
    public class CardBehaviour: MonoBehaviour {
        public Card card;
        public CardView cardView;
        public GameObject cardObject;

        public void Init(GameObject cardObject, Card card) {
            this.card = card;
            this.cardView = this.GetComponent<CardView>();
            this.cardView.SetCardView(card);
            this.cardObject = cardObject;
        }

        public void DestroySelf() {
            CardManager.Instance.RemoveCard(this.cardObject);
            Destroy(this.cardObject);
        }
    }
}
