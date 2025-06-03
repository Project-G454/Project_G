using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Entities;
using UnityEngine;
using UnityEngine.UI;

class BattleRewardsManager: MonoBehaviour, IManager, IEntryManager {
    public static BattleRewardsManager Instance { get; private set; }
    public GameObject playerRewardPanelPrefab;
    public Transform panelContainer;
    public Button confirmButton; // 顯示等待其他人完成

    private List<PlayerRewardPanel> _panels = new();
    private List<Entity> _players;

    private void Awake() {
        Instance = this;
    }

    public void Reset() {
        Instance = null;
    }

    void Start() {
        Entry();
    }

    public void Entry() {
        List<Entity> Players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
        ShowRewards(Players);
    }

    public void Init() {
    }

    public void ShowRewards(List<Entity> players) {
        _players = players;
        foreach (Transform child in panelContainer) Destroy(child.gameObject);
        _panels.Clear();

        foreach (var player in players) {
            var panelGO = Instantiate(playerRewardPanelPrefab, panelContainer);
            var panel = panelGO.GetComponent<PlayerRewardPanel>();
            var cardOptions = GetRandomcards();
            panel.Setup(player, cardOptions, OnPlayerPickedCard);
            _panels.Add(panel);
        }
        confirmButton.onClick.AddListener(() => OnConfirm());
        confirmButton.interactable = false;
    }

    public List<int> GetRandomcards() {
        var cardIds = new List<int> {1, 2, 3, 4, 5, 6, 8, 9, 10};
        var rng = new System.Random();
        return cardIds.OrderBy(x => rng.Next()).Take(3).ToList();
    }

    private void OnPlayerPickedCard(PlayerRewardPanel panel, int? cardId) {
        var playerIndex = _panels.IndexOf(panel);
        if (playerIndex >= 0 && cardId != null) {
            _players[playerIndex].deckManager.deck.Add((int)cardId);
        }

        confirmButton.interactable = AllPlayersPicked();
    }

    private void OnConfirm() {
        for (int i = 0; i < _panels.Count; i++) {
            var selectedCard = _panels[i].GetSelectedCard();
            if (selectedCard != null) {
                _players[i].deckManager.deck.Add((int) selectedCard);
            }
        }

        Reset();
        SceneTransitionHelper.LoadWorldMapScene();
    }

    private bool AllPlayersPicked() {
        foreach (var panel in _panels) {
            if (!panel.HasPicked()) return false;
        }
        return true;
    }
}
