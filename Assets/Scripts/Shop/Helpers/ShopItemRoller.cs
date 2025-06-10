using System.Collections.Generic;
using Shop.Models;

namespace Shop.Helpers {
    public static class ShopItemRoller {
        public static ShopItemType RollRandomTypes() {
            Dictionary<ShopItemType, float> probability = new Dictionary<ShopItemType, float>() {
                { ShopItemType.Card, 0.5f }, // 50% chance for card items
                { ShopItemType.Heal, 0.5f } // 50% chance for potion items
            };

            ShopItemType type = ShopItemType.Unset;
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            float cumulativeProbability = 0f;
            foreach (var item in probability) {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability) {
                    type = item.Key;
                    break;
                }
            }

            return type;
        }

        public static ShopItemRarity RollRandomRarity() {
            Dictionary<ShopItemRarity, float> rarityProbability = new Dictionary<ShopItemRarity, float>() {
                { ShopItemRarity.Common, 0.45f }, // 45% chance for common items
                { ShopItemRarity.Uncommon, 0.25f }, // 25% chance for uncommon items
                { ShopItemRarity.Rare, 0.15f },   // 15% chance for rare items
                { ShopItemRarity.Epic, 0.10f },  // 10% chance for epic items
                { ShopItemRarity.Legendary, 0.05f } // 5% chance for legendary items
            };

            ShopItemRarity rarity = ShopItemRarity.Unset;
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            float cumulativeProbability = 0f;
            foreach (var item in rarityProbability) {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability) {
                    rarity = item.Key;
                    break;
                }
            }
            return rarity;
        }
    }
}
