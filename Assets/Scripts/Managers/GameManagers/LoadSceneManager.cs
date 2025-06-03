using System;
using System.Collections;
using Core.Interfaces;
using Core.Managers.WorldMap;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldMap;
using WorldMap.Models;

namespace Core.Managers {
    public class LoadSceneManager: MonoBehaviour, IManager {
        public static LoadSceneManager Instance;
        private AsyncOperation _asyncOperation;
        private GlobalUIManager _globalUIManager;
        private string _loadingSceneName;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            _globalUIManager = GlobalUIManager.Instance;
        }

        public void Start() {
            Init();
        }

        public void Reset() { }

        private void _LoadScene(string sceneName) {
            _loadingSceneName = sceneName;
            _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            _asyncOperation.allowSceneActivation = true;

            _asyncOperation.completed += _ => {
                switch (sceneName) {
                    case "WorldMap":
                        WorldMapManager.Instance.Entry();
                        break;
                    case "SceneCiel":
                        BattleManager.Instance.Entry();
                        break;
                    case "Shop":
                        ShopManager.Instance.Entry();
                        break;
                }
                _loadingSceneName = null;
                _asyncOperation.allowSceneActivation = false;
            };
        }

        public void LoadBattleScene(MapNode node) {
            if (_globalUIManager == null) Init();
            _globalUIManager.stageAlertUI.Show(
                node,
                () => _LoadScene("SceneCiel")
            );
        }

        public void LoadShopScene(MapNode node) {
            if (_globalUIManager == null) Init();
            _globalUIManager.stageAlertUI.Show(
                node,
                () => _LoadScene("Shop")
            );
        }

        public void LoadWorldMapScene() {
            if (_globalUIManager == null) Init();
            _globalUIManager.confirmAlertUI.Show(
                "Exit",
                "Return to world map?",
                () => _LoadScene("WorldMap")
            );
        }

        public void LoadBattleRewardsScene() {
            if (_globalUIManager == null) Init();
            _LoadScene("BattleRewards");
        }
    }
}
