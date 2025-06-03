using Cards.Data;
using UnityEngine;

namespace Shop.Models {
    [CreateAssetMenu(fileName = "Card", menuName = "Shop/Card")]
    public class ShopCardSO: ShopItemSO {
        public override ShopItemType itemType => ShopItemType.Card;
        public CardData card;
    }
}
