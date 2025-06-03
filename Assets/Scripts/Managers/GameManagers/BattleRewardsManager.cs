using System.Collections.Generic;
using System.Linq;
using Cards.Data;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Managers;
using Core.UI;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers {
    class BattleRewardsManager: MonoBehaviour, IManager, IEntryManager {
        public static BattleRewardsManager Instance { get; private set; }
        private DescriptionManager _descriptionManager;
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
            _descriptionManager = ManagerHelper.RequireManager(DescriptionManager.Instance);
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
                panel.Setup(player, OnPickedCard);
                _panels.Add(panel);
            }
            confirmButton.onClick.AddListener(() => OnConfirm());
            confirmButton.interactable = false;
        }

        private void OnPickedCard(PlayerRewardPanel panel, CardData cardData) {
            var playerIndex = _panels.IndexOf(panel);
            if (playerIndex >= 0 && cardData != null) {
                _players[playerIndex].deckManager.deck.Add(cardData.id);
            }

            confirmButton.interactable = AllPlayersPicked();
        }

        private void OnConfirm() {
            for (int i = 0; i < _panels.Count; i++) {
                var selectedCard = _panels[i].GetSelectedCard();
                if (selectedCard != null) {
                    _players[i].deckManager.deck.Add(selectedCard.id);
                }
            }

            Reset();
            LoadSceneManager.Instance.LoadWorldMapScene();
        }

        private bool AllPlayersPicked() {
            foreach (var panel in _panels) {
                if (!panel.HasPicked()) return false;
            }
            return true;
        }
    }
}
