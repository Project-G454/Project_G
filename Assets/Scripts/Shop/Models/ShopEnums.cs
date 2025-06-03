namespace Shop.Models {
    public enum ShopItemType {
        Unset,
        Card,
        Heal
    }

    public enum ShopItemRarity {
        Unset,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum ShopItemState {
        Available,
        SoldOut,
        Locked
    }
}
