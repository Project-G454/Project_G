using Cards;
using Core.Managers;
using Core.UI;
using Descriptions;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Shop.Handlers {
    public class ShopCardStateHandler: MonoBehaviour, IPointerClickHandler {
        private CardBehaviour _cardBehaviour;
        private CardActiveUI _cardActiveUI;

        public void Init() {
            _cardBehaviour = GetComponent<CardBehaviour>();
            _cardActiveUI = GlobalUIManager.Instance.cardActiveUI;
        }

        public void Start() {
            Init();
        }

        public void OnPointerClick(PointerEventData eventData) {
            _cardActiveUI.Show(_cardBehaviour.card);
        }
    }
}
