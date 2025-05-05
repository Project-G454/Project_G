using Core.Managers.Cards;
using UnityEngine;

namespace Cards {
    /// <summary>
    /// 將後端、外觀及物件綁定在一起。
    /// </summary>
    /// <remarks>
    /// 每張卡片都應有一個 CardBehaviour。
    /// </remarks>
    public class CardBehaviour: MonoBehaviour {
        public Card card;               // 卡片後端邏輯、資料
        public CardView cardView;       // 卡片外觀顯示
        public GameObject cardObject;   // 卡片在遊戲中的物件

        /// <summary>
        /// 將卡片後端、外觀及物件綁定在一起。
        /// </summary>
        /// <param name="cardObject">卡片物件</param>
        /// <param name="card">卡片資料</param>
        public void Init(GameObject cardObject, Card card) {
            this.card = card;
            this.cardView = this.GetComponent<CardView>();
            this.cardView.SetCardView(card);
            this.cardObject = cardObject;
        }

        /// <summary>
        /// 將卡片物件刪除，並從 CardManager 取消註冊。
        /// </summary>
        public void DestroySelf() {
            CardManager.Instance.RemoveCard(cardObject);
            Destroy(cardObject);
        }
    }
}
