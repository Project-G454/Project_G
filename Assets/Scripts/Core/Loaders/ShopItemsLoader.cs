using System.Collections.Generic;
using System.Linq;
using Shop.Models;
using UnityEngine;

namespace Core.Loaders.Shop {
    public static class ShopItemsLoader {
        public static List<ShopCardSO> LoadShopCard() {
            return Resources.LoadAll<ShopCardSO>("Shop/Card Items").ToList();
        }

        public static List<ShopHealSO> LoadShopHeal() {
            return Resources.LoadAll<ShopHealSO>("Shop/Heal Items").ToList();
        }
    }
}
