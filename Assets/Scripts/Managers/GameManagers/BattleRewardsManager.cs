using System.Collections.Generic;
using Core.Game;
using Core.Interfaces;
using Reward;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers {
    class BattleRewardsManager: MonoBehaviour, IManager, IEntryManager {
        public static BattleRewardsManager Instance { get; private set; }
        public GameObject playerRewardPanelPrefab;
        public Transform panelContainer;
        public Button confirmButton; // 顯示等待其他人完成

        private List<PlayerRewardPanel> _panels = new();
        private List<GamePlayerState> _players;

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
            List<GamePlayerState> Players = PlayerStateManager.Instance.GetAllPlayer();
            ShowRewards(Players);
        }

        public void Init() {
        }

        public void ShowRewards(List<GamePlayerState> players) {
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

        private void OnPickedCard() {
            confirmButton.interactable = AllPlayersPicked();
        }

        private void OnConfirm() {
            LoadSceneManager.Instance.LoadWorldMapScene_BattleReward(AddCardToDeck);
        }

        private void AddCardToDeck() {
            for (int i = 0; i < _panels.Count; i++) {
                var selectedCard = _panels[i].GetSelectedCard();
                if (selectedCard != null) {
                    _players[i].deck.Add(selectedCard.cardData.id);
                }
            }

            Reset();
        }

        private bool AllPlayersPicked() {
            foreach (var panel in _panels) {
                if (!panel.HasPicked()) return false;
            }
            return true;
        }
    }
}
