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

        public void Init(ShopCardSO data, CardData cardData = null) {
            _cardItem = data;
            base.Init();

            if (cardData != null) data.card = cardData;
            Card card = new Card(cardData);
            CardBehaviour cb = cardObj.GetComponent<CardBehaviour>();
            cb.Init(cardObj, card);
        }

        public override bool Buy() {
            if (!base.Buy()) return false;
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            List<int> deck = new List<int>(player.deck) { _cardItem.card.id };
            playerStateManager.SetDeck(player.playerId, deck);
            return true;
        }
    }
}
