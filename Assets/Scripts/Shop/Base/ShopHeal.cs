using Shop.Models;
using TMPro;
using UnityEngine.UI;

namespace Shop.Items {
    public class ShopHeal: ShopItem {
        public override ShopItemSO item => _healItem;
        private ShopHealSO _healItem;
        public Image icon;
        public TMP_Text title;

        public void Init(ShopHealSO data) {
            _healItem = data;
            base.Init();

            icon.sprite = data.icon;
            title.text = $"Heal {data.healingAmount} HP";
        }

        public override void Buy() {
            // Implement logic to handle the purchase of the card
            // This could include checking if the player has enough currency, updating inventory, etc.
        }
    }
}
