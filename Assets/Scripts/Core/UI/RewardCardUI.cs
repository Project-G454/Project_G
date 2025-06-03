using System;
using Cards;
using Cards.Data;
using Core.Managers;
using Shop.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Reward {
    public class RewardCard: MonoBehaviour, IPointerClickHandler {
        public Button cardButton;
        private CardData _cardData;
        private Card _card;
        public GameObject cardObj;
        private Action<CardData> _onCardSelectedCallBack;

        public void Init(CardData data, Action<CardData> onCardSelectedCallBack) {
            _cardData = data;

            _card = new Card(data);
            CardBehaviour cb = cardObj.GetComponent<CardBehaviour>();
            cb.Init(cardObj, _card);

            _onCardSelectedCallBack = onCardSelectedCallBack;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                _onCardSelectedCallBack?.Invoke(_cardData);
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                GlobalUIManager.Instance.cardActiveUI.Show(_card);
            }
        }
    }
}
