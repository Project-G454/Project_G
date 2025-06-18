using System.Collections.Generic;
using UnityEngine;
using Core.Interfaces;
using Shop.Models;
using Core.Loaders.Shop;
using System.Linq;
using Shop.Items;
using Core.Helpers;
using UnityEngine.UI;
using Core.Game;
using TMPro;
using System.Collections;
using System;
using Shop.Helpers;
using Shop.Factories;
using Entities;

namespace Core.Managers {
    public class ShopManager: MonoBehaviour, IManager, IEntryManager {
        public static ShopManager Instance;
        public Transform itemsParent;
        public GameObject cardItemPrefab;
        public GameObject healItemPrefab;
        public Button worldMapBtn;
        public TMP_Text goldText;
        public int playerId;
        private DescriptionManager _descriptionManager;
        private LoadSceneManager _loadSceneManager;
        private PlayerStateManager _playerStateManager;
        private bool _isInit = false;
        private List<GameObject> items = new();
        private bool _isPlayerEnd = false;
        private int _endedPlayerCount = 0;

        private void Awake() {
            Instance = this;
        }

        public void Reset() {
            Instance = null;
        }

        public void Init() {
            // Initialize shop card states or any other setup needed
            _descriptionManager = ManagerHelper.RequireManager(DescriptionManager.Instance);
            _loadSceneManager = ManagerHelper.RequireManager(LoadSceneManager.Instance);
            _playerStateManager = ManagerHelper.RequireManager(PlayerStateManager.Instance);
        }

        private void _SetEndAction() {
            Action action;
            if (_endedPlayerCount == _playerStateManager.GetAllPlayer().Count - 1) {
                action = () => _loadSceneManager.LoadWorldMapScene(true);
            }
            else action = () => _isPlayerEnd = true;
            worldMapBtn.onClick.RemoveAllListeners();
            worldMapBtn.onClick.AddListener(action.Invoke);
        }

        public void Entry() {
            if (_isInit) return;
            Init();
            StartCoroutine(_PlayerLoop());
        }

        public void UpdateShopState() {
            GamePlayerState playerData = _playerStateManager.GetPlayer(playerId);

            goldText.text = playerData.gold.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(goldText.rectTransform);

            foreach (var itemObj in items) {
                ShopItem item = itemObj.GetComponent<ShopItem>();
                if (item == null) continue;
                item.CheckState();
            }
        }

        private IEnumerator _PlayerLoop() {
            foreach (var player in _playerStateManager.GetAllPlayer()) {
                _SetEndAction();
                _LoadPlayerData(player.playerId);
                _Roll(3, player.entityClass);
                UpdateShopState();
                _isInit = true;

                yield return new WaitUntil(() => _isPlayerEnd);
                _ClearShopItems();
                _isInit = false;
                _isPlayerEnd = false;
                _endedPlayerCount += 1;
            }
        }

        private void _LoadPlayerData(int playerId) {
            this.playerId = playerId;
        }

        private void _Roll(int times = 3, EntityClasses classes = EntityClasses.UNSET) {
            _ClearShopItems();
            for (int i = 0; i < times; i++) {
                ShopItemType type = ShopItemRoller.RollRandomTypes();
                ShopItemRarity rarity = ShopItemRoller.RollRandomRarity();
                items.Add(ShopItemCreater.CreateItem(type, rarity, classes));
            }
        }

        private void _ClearShopItems() {
            List<GameObject> destoryItems = new(items);
            foreach (var item in destoryItems) {
                items.Remove(item);
                Destroy(item);
            }
        }
    }
}
