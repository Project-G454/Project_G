using Core.Game;
using Core.Managers;
using DG.Tweening;
using Shop.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shop.Items {
    public abstract class ShopItem: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public ShopItemState state = ShopItemState.Available;
        public Button buyButton;
        public TMP_Text btnText;
        public GameObject soldOutPanel;
        protected ShopManager shopManager;
        protected PlayerStateManager playerStateManager;
        private Vector3 _oriScale;

        public abstract ShopItemSO item { get; }

        public virtual void Init() {
            shopManager = ShopManager.Instance;
            playerStateManager = PlayerStateManager.Instance;
            btnText.text = item.price.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(btnText.rectTransform);
            soldOutPanel.SetActive(false);
            _oriScale = transform.localScale;
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
            buyButton.onClick.RemoveAllListeners();
            Flip(0.3f);
        }

        private void _Lock() {
            this.state = ShopItemState.SoldOut;
        }

        public void Flip(float duration = 0.5f){
            RectTransform rt = GetComponent<RectTransform>();
            Sequence flipSeq = DOTween.Sequence();

            // 前半段：縮小 X 軸到 0（像是翻到一半）
            flipSeq.Append(rt.DOScaleX(0f, duration / 2).SetEase(Ease.InOutQuad));

            // 替換內容（例如切換圖片）
            flipSeq.AppendCallback(() => {
                // TODO：切換卡牌正反面，例如換圖片、換文字
                soldOutPanel.SetActive(true);
            });

            // 後半段：從 0 拉回到 1，完成翻牌
            flipSeq.Append(rt.DOScaleX(1f, duration / 2).SetEase(Ease.InOutQuad));
        }

        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(_oriScale*1.1f, 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOScale(_oriScale, 0.3f);
        }
    }
}
