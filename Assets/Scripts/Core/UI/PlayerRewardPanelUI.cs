// PlayerRewardPanel.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cards.Data;
using Entities.Categories;
using Entities;

public class PlayerRewardPanel: MonoBehaviour {
    //public TextMeshProUGUI playerNameText;
    public Image avatar;
    public Button[] cardButtons; // Expected size: 3
    public Button skipButton;
    //public TextMeshProUGUI pickStatusText;

    private int? _selectedCard = null;
    private bool _isSkip = false;

    private List<int> _cards;
    private Action<PlayerRewardPanel, int?> _onPickCallback;
    private bool _hasPicked = false;

    public void Setup(Entity player, List<int> cardOptions, Action<PlayerRewardPanel, int?> onPickCallback) {
        //playerNameText.text = player.entityName;
        avatar.sprite = player.avatar;
        _onPickCallback = onPickCallback;
        _cards = cardOptions;
        for (int i = 0; i < cardButtons.Length; i++) {
            int index = i;
            if (i < _cards.Count) {
                cardButtons[i].gameObject.SetActive(true);
                cardButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _cards[i].ToString();
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() => OnCardSelected(_cards[index]));
            }
            else {
                cardButtons[i].gameObject.SetActive(false);
            }
        }

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() => OnSkipSelected());
        //pickStatusText.text = "尚未選擇";
        _hasPicked = false;
    }

    private void OnCardSelected(int selectedCard) {
        if (_selectedCard == selectedCard) {
            _hasPicked = false;
        }
        else {
            _hasPicked = true;
            _selectedCard = selectedCard;
            _isSkip = false;
            UpdateCardHighlight();
        }

        _onPickCallback?.Invoke(this, _selectedCard);
    }

    private void OnSkipSelected() {
        _hasPicked = true;
        _selectedCard = null;
        _isSkip = true;
        UpdateCardHighlight();

        _onPickCallback?.Invoke(this, _selectedCard);
    }

    private void UpdateCardHighlight() {

    }

    public bool HasPicked() => _hasPicked;
    
    public int? GetSelectedCard() => _isSkip ? null : _selectedCard;
}
