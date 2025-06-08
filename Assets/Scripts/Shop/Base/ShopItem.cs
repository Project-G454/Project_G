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

        public virtual void CheckState() {
            buyButton.onClick.RemoveAllListeners();

            if (!IsPlayerCanBuy()) {
                Lock();
                return;
            }

            buyButton.onClick.AddListener(() => {
                if (!Buy()) return;
                Sold();
                shopManager.UpdateShopState();
            });
        }

        private bool IsPlayerCanBuy() {
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            return state == ShopItemState.Available && player.gold >= item.price;
        }

        public virtual bool Buy() {
            GamePlayerState player = playerStateManager.GetPlayer(shopManager.playerId);
            if (!IsPlayerCanBuy()) return false;
            playerStateManager.ModifyGold(player.playerId, -item.price);
            return true;
        }

        private void Sold() {
            Debug.Log("Sold!");
            this.state = ShopItemState.SoldOut;
        }

        private void Lock() {
            this.state = ShopItemState.SoldOut;
        }
    }
}
