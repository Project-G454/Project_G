using Core.Game;
using Core.Managers;
using Shop.Models;
using TMPro;
using UnityEngine;
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

        public override bool Buy() {
            if (!base.Buy()) return false;
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            playerStateManager.ModifyHP(player.playerId, _healItem.healingAmount);
            return true;
        }
    }
}
