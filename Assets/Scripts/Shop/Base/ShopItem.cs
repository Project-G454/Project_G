using Shop.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Items {
    public abstract class ShopItem: MonoBehaviour {
        public ShopItemState state = ShopItemState.Available;
        public Button buyButton;
        public TMP_Text btnText;

        public abstract ShopItemSO item { get; }

        public virtual void Init() {
            buyButton.onClick.AddListener(Buy);
            btnText.text = item.price.ToString();
        }

        public virtual void CheckState(int money) {
            if (money < item.price) {
                state = ShopItemState.Locked;
            }
        }

        public abstract void Buy();
    }
}
