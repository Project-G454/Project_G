using Core.Game;
using Core.Managers;
using Shop.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Items {
    public abstract class ShopItem: MonoBehaviour {
        public ShopItemState state = ShopItemState.Available;
        public Button buyButton;
        public TMP_Text btnText;
        protected ShopManager shopManager;
        protected PlayerStateManager playerStateManager;

        public abstract ShopItemSO item { get; }

        public virtual void Init() {
            shopManager = ShopManager.Instance;
            playerStateManager = PlayerStateManager.Instance;
            btnText.text = item.price.ToString();
            CheckState();
        }

        public void CheckState() {
            buyButton.onClick.RemoveAllListeners();

            if (!_IsPlayerCanBuy()) {
                _Lock();
                return;
            }

            buyButton.onClick.AddListener(() => {
                if (!Buy()) return;
                _Sold();
                shopManager.UpdateShopState();
            });
        }

        private bool _IsPlayerCanBuy() {
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            return state == ShopItemState.Available && player.gold >= item.price;
        }

        public virtual bool Buy() {
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            if (!_IsPlayerCanBuy()) return false;
            playerStateManager.ModifyGold(player.playerId, -item.price);
            return true;
        }

        private void _Sold() {
            this.state = ShopItemState.SoldOut;
        }

        private void _Lock() {
            this.state = ShopItemState.SoldOut;
        }
    }
}
