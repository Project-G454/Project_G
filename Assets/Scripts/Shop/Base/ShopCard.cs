using System.Collections.Generic;
using Cards;
using Cards.Data;
using Core.Game;
using Core.Managers.Deck;
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

        public override bool Buy() {
            if (!base.Buy()) return false;
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            List<int> deck = new List<int>(player.deck) { _cardItem.card.id };
            Debug.Log($"Old Deck: {string.Join(", ", player.deck)}");
            playerStateManager.SetDeck(player.playerId, deck);
            Debug.Log($"New Deck: {string.Join(", ", player.deck)}");
            return true;
        }
    }
}
