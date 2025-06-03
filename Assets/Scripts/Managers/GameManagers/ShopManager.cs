using System.Collections.Generic;
using UnityEngine;
using Core.Interfaces;
using Core.UI;
using Shop.Models;
using UnityEngine.SocialPlatforms;
using Core.Loaders.Shop;
using Unity.VisualScripting;
using System.Linq;
using Shop.Items;
using Core.Helpers;
using UnityEngine.UI;


namespace Core.Managers {
    public class ShopManager: MonoBehaviour, IManager, IEntryManager {
        public static ShopManager Instance;
        public Transform itemsParent;
        public GameObject cardItemPrefab;
        public GameObject healItemPrefab;
        public Button worldMapBtn;
        private DescriptionManager _descriptionManager;
        private LoadSceneManager _loadSceneManager;
        private bool _isInit = false;


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
            worldMapBtn.onClick.AddListener(_loadSceneManager.LoadWorldMapScene);
            _isInit = true;
        }

        public void Entry() {
            if (_isInit) return;
            Init();
            Roll();
        }

        void Start() {
            Entry();
        }

        public void Roll(int times = 3) {
            for (int i = 0; i < times; i++) {
                ShopItemType type = RollRandomTypes();
                ShopItemRarity rarity = RollRandomRarity();
                List<GameObject> items = new();

                // 根據類型載入資料
                switch (type) {
                    case ShopItemType.Card:
                        items.Add(CreateCardItem(rarity));
                        break;
                    case ShopItemType.Heal:
                        items.Add(CreateHealItem(rarity));
                        break;
                }
            }
        }

        public GameObject CreateCardItem(ShopItemRarity rarity) {
            List<ShopCardSO> dataList = ShopItemsLoader.LoadShopCard();
            dataList = dataList.Where(data => data.itemRarity == rarity).ToList();
            ShopCardSO data = dataList[Random.Range(0, dataList.Count)];

            GameObject newItem = Instantiate(cardItemPrefab, itemsParent);
            ShopCard item = newItem.GetComponent<ShopCard>();
            item.Init(data);
            return newItem;
        }

        public GameObject CreateHealItem(ShopItemRarity rarity) {
            List<ShopHealSO> dataList = ShopItemsLoader.LoadShopHeal();
            dataList = dataList.Where(data => data.itemRarity == rarity).ToList();
            ShopHealSO data = dataList[Random.Range(0, dataList.Count)];

            GameObject newItem = Instantiate(healItemPrefab, itemsParent);
            ShopHeal behaviour = newItem.GetComponent<ShopHeal>();
            behaviour.Init(data);
            return newItem;
        }

        public ShopItemType RollRandomTypes() {
            Dictionary<ShopItemType, float> probability = new Dictionary<ShopItemType, float>() {
                { ShopItemType.Card, 0.5f }, // 50% chance for card items
                { ShopItemType.Heal, 0.5f } // 50% chance for potion items
            };

            ShopItemType type = ShopItemType.Unset;
            float randomValue = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;
            foreach (var item in probability) {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability) {
                    type = item.Key;
                    break;
                }
            }

            return type;
        }

        public ShopItemRarity RollRandomRarity() {
            Dictionary<ShopItemRarity, float> rarityProbability = new Dictionary<ShopItemRarity, float>() {
                { ShopItemRarity.Common, 0.45f }, // 45% chance for common items
                { ShopItemRarity.Uncommon, 0.25f }, // 25% chance for uncommon items
                { ShopItemRarity.Rare, 0.15f },   // 15% chance for rare items
                { ShopItemRarity.Epic, 0.10f },  // 10% chance for epic items
                { ShopItemRarity.Legendary, 0.05f } // 5% chance for legendary items
            };

            ShopItemRarity rarity = ShopItemRarity.Unset;
            float randomValue = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;
            foreach (var item in rarityProbability) {
                cumulativeProbability += item.Value;
                if (randomValue <= cumulativeProbability) {
                    rarity = item.Key;
                    break;
                }
            }

            return rarity;
        }
    }
}
