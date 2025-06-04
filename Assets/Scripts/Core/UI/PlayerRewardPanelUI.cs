// PlayerRewardPanel.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cards.Data;
using Entities.Categories;
using Entities;
using Shop.Models;
using Core.Loaders.Shop;
using Shop.Items;
using Reward;
using Core.Loaders.Cards;
using UnityEngine.UI;

namespace Core.UI {
    public class PlayerRewardPanel: MonoBehaviour {
        public Transform itemsParent;
        public GameObject cardItemPrefab;
        //public TextMeshProUGUI playerNameText;
        public Image avatar;
        public Button skipButton;
        //public TextMeshProUGUI pickStatusText;

        private CardData _selectedCard = null;
        private bool _isSkip = false;
        private Action<PlayerRewardPanel, CardData> _onPickedCardCallBack;
        private bool _hasPicked = false;

        public void Setup(Entity player, Action<PlayerRewardPanel, CardData> onPickedCardCallBack) {
            //playerNameText.text = player.entityName;
            avatar.sprite = player.avatar;
            _onPickedCardCallBack = onPickedCardCallBack;

            foreach (Transform child in itemsParent)
                Destroy(child.gameObject);

            for (int i = 0; i < 3; i++) {
                var slot = CreateCardItem();
            }

            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() => OnSkipSelected());
            _hasPicked = false;
        }

        public GameObject CreateCardItem() {
            List<CardData> dataList = CardDataLoader.LoadAll();
            CardData data = dataList[UnityEngine.Random.Range(0, dataList.Count)];

            GameObject newItem = Instantiate(cardItemPrefab, itemsParent);
            RewardCard item = newItem.GetComponent<RewardCard>();
            item.Init(data, OnCardSelected);
            return newItem;
        }

        private void OnCardSelected(CardData selectedCard) {
            if (_selectedCard == selectedCard) {
                _hasPicked = false;
            }
            else {
                _hasPicked = true;
                _selectedCard = selectedCard;
                _isSkip = false;
                UpdateCardHighlight();
            }

            _onPickedCardCallBack?.Invoke(this, _selectedCard);
        }

        private void OnSkipSelected() {
            _hasPicked = true;
            _selectedCard = null;
            _isSkip = true;
            UpdateCardHighlight();

            _onPickedCardCallBack?.Invoke(this, _selectedCard);
        }

        private void UpdateCardHighlight() {

        }

        public bool HasPicked() => _hasPicked;

        public CardData GetSelectedCard() => _isSkip ? null : _selectedCard;
    }
}
