using Cards;
using Cards.Data;
using Shop.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Items {
    public class ShopCard: ShopItem {
        public override ShopItemSO item => _cardItem;
        private ShopCardSO _cardItem;
        public GameObject cardObj;

        public void Init(ShopCardSO data) {
            _cardItem = data;
            base.Init();
            
            Card card = new Card(data.card);
            CardBehaviour cb = cardObj.GetComponent<CardBehaviour>();
            cb.Init(cardObj, card);
        }

        public override void Buy() {
            // Implement logic to handle the purchase of the card
            // This could include checking if the player has enough currency, updating inventory, etc.
        }
    }
}
