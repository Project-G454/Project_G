using UnityEngine;

namespace Shop.Models {
    public abstract class ShopItemSO: ScriptableObject {
        public abstract ShopItemType itemType { get; }
        public ShopItemRarity itemRarity = ShopItemRarity.Unset;
        public int price = 0;
    }
}
