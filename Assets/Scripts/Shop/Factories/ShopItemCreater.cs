using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.Data;
using Core.Loaders.Cards;
using Core.Loaders.Shop;
using Core.Managers;
using Entities;
using Shop.Items;
using Shop.Models;
using UnityEngine;

namespace Shop.Factories {
    public static class ShopItemCreater {
        public static GameObject CreateItem(ShopItemType itemType, ShopItemRarity rarity, EntityClasses classes = EntityClasses.UNSET) {
            GameObject newObject = itemType switch {
                ShopItemType.Card => _CreateCardItem(rarity, classes),
                ShopItemType.Heal => _CreateHealItem(rarity),
                _ => null
            };
            return newObject;
        }

        private static GameObject _CreateCardItem(ShopItemRarity rarity, EntityClasses classes) {
            List<ShopCardSO> dataList = ShopItemsLoader.LoadShopCard();
            dataList = dataList.Where(data => data.itemRarity == rarity).ToList();
            ShopCardSO data = dataList[Random.Range(0, dataList.Count)];

            GameObject prefab = ShopManager.Instance.cardItemPrefab;
            Transform parent = ShopManager.Instance.itemsParent;
            GameObject newItem = GameObject.Instantiate(prefab, parent);
            ShopCard item = newItem.GetComponent<ShopCard>();

            List<CardData> cards = CardDataLoader.LoadByClass(classes);
            cards = cards.Where(e => (int)e.rarity == (int)rarity).ToList();

            if (cards.Count == 0) {
                GameObject.DestroyImmediate(newItem);
                return _CreateHealItem(rarity);
            }

            CardData cardData = cards[Random.Range(0, cards.Count)];

            item.Init(data, cardData);
            return newItem;
        }

        private static GameObject _CreateHealItem(ShopItemRarity rarity) {
            List<ShopHealSO> dataList = ShopItemsLoader.LoadShopHeal();
            dataList = dataList.Where(data => data.itemRarity == rarity).ToList();
            ShopHealSO data = dataList[Random.Range(0, dataList.Count)];

            GameObject prefab = ShopManager.Instance.healItemPrefab;
            Transform parent = ShopManager.Instance.itemsParent;
            GameObject newItem = GameObject.Instantiate(prefab, parent);
            ShopHeal behaviour = newItem.GetComponent<ShopHeal>();
            behaviour.Init(data);
            return newItem;
        }
    }
}
