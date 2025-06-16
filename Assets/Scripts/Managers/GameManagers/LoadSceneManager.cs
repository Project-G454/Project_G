using System;
using System.Collections;
using Core.Handlers;
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
        public TransitionHandler transHandler;

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
            if (transHandler != null) {
                StartCoroutine(_LoadWithTransition(sceneName));
            }
            else _NormalLoad(sceneName);
        }

        private IEnumerator _LoadWithTransition(string sceneName) {
            yield return StartCoroutine(transHandler.StartTransition());
            _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            _asyncOperation.allowSceneActivation = true;
            _RunEntry(sceneName);
            yield return new WaitUntil(() => _asyncOperation.isDone);
            yield return StartCoroutine(transHandler.EndTransition());
        }

        private void _NormalLoad(string sceneName) {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            _asyncOperation.allowSceneActivation = true;
            _RunEntry(sceneName);
        }

        private void _RunEntry(string sceneName) {
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

        public void LoadWorldMapScene(bool showAlert = true) {
            if (!showAlert) {
                _LoadScene("WorldMap");
                return;
            }
            if (_globalUIManager == null) Init();
            _globalUIManager.confirmAlertUI.Show(
                "Exit",
                "Return to world map?",
                () => _LoadScene("WorldMap")
            );
        }

        public void LoadWorldMapScene_BattleReward(Action OnConfirm) {
            if (_globalUIManager == null) Init();
            _globalUIManager.confirmAlertUI.Show(
                "Exit",
                "Return to world map?",
                () => {
                    OnConfirm?.Invoke();
                    _LoadScene("WorldMap");
                }
            );
        }

        public void LoadBattleRewardsScene() {
            if (_globalUIManager == null) Init();
            _LoadScene("BattleRewards");
        }
    }
}
