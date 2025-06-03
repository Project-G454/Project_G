using Cards.Data;
using UnityEngine;

namespace Shop.Models {
    [CreateAssetMenu(fileName = "Heal", menuName = "Shop/Heal")]
    public class ShopHealSO: ShopItemSO {
        public override ShopItemType itemType => ShopItemType.Heal;
        public Sprite icon;
        public int healingAmount;
    }
}
