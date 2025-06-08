using System.Collections.Generic;
using System.Linq;
using Core.Loaders.Shop;
using Core.Managers;
using Shop.Items;
using Shop.Models;
using UnityEngine;

namespace Shop.Factories {
    public static class ShopItemCreater {
        public static GameObject CreateItem(ShopItemType itemType, ShopItemRarity rarity) {
            GameObject newObject = itemType switch {
                ShopItemType.Card => _CreateCardItem(rarity),
                ShopItemType.Heal => _CreateHealItem(rarity),
                _ => null
            };
            return newObject;
        }

        private static GameObject _CreateCardItem(ShopItemRarity rarity) {
            List<ShopCardSO> dataList = ShopItemsLoader.LoadShopCard();
            dataList = dataList.Where(data => data.itemRarity == rarity).ToList();
            ShopCardSO data = dataList[Random.Range(0, dataList.Count)];

            GameObject prefab = ShopManager.Instance.cardItemPrefab;
            Transform parent = ShopManager.Instance.itemsParent;
            GameObject newItem = GameObject.Instantiate(prefab, parent);
            ShopCard item = newItem.GetComponent<ShopCard>();
            item.Init(data);
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
