using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Game;
using Reward.Factories;

namespace Reward {
    public class PlayerRewardPanel: MonoBehaviour {
        public Transform itemsParent;
        public GameObject cardItemPrefab;
        public Image avatar;
        public Button skipButton;
        private RewardCard _selectedCard;
        private bool _isSkip = false;
        private Action _onPickedCardCallBack;
        private bool _hasPicked = false;
        private List<RewardCard> _cardOptions = new();

        public void Setup(GamePlayerState player, Action onPickedCardCallBack) {
            avatar.sprite = player.avatar;
            _onPickedCardCallBack = onPickedCardCallBack;

            foreach (Transform child in itemsParent)
                Destroy(child.gameObject);

            for (int i = 0; i < 3; i++) {
                var slot = RewardCardFactory.CreateRewardCard(cardItemPrefab, itemsParent, OnCardSelected);
                var RewardCard = slot.GetComponent<RewardCard>();
                _cardOptions.Add(RewardCard);
            }

            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() => OnSkipSelected());
            _hasPicked = false;
        }

        private void OnCardSelected(RewardCard selectedCard) {
            if (_selectedCard == selectedCard) {
                _hasPicked = false;
            }
            else {
                _hasPicked = true;
                _selectedCard = selectedCard;
                _isSkip = false;
                UpdateCardsHighlight();
            }

            _onPickedCardCallBack?.Invoke();
        }

        private void OnSkipSelected() {
            _hasPicked = true;
            _selectedCard = null;
            _isSkip = true;
            UpdateCardsHighlight();

            _onPickedCardCallBack?.Invoke();
        }

        private void UpdateCardsHighlight() {
            foreach (var card in _cardOptions) {
                card.AnimateFocus(_selectedCard == card);
            }
        }

        public bool HasPicked() => _hasPicked;

        public RewardCard GetSelectedCard() => _isSkip ? null : _selectedCard;
    }
}
