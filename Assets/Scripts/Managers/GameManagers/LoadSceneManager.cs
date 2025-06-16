using System;
using System.Collections;
using System.Collections.Generic;
using Core.Handlers;
using Core.Interfaces;
using Core.Managers.WorldMap;
using Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldMap;
using WorldMap.Models;
using Entities.Factories;

namespace Core.Managers {
    public class LoadSceneManager: MonoBehaviour, IManager {
        public static LoadSceneManager Instance;
        private AsyncOperation _asyncOperation;
        private GlobalUIManager _globalUIManager;
        private string _loadingSceneName;
        public TransitionHandler transHandler;
        private List<EntityData> _entityDatas;

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
                        if (_entityDatas.Count > 0) BattleManager.Instance.EntryWithEntityData(_entityDatas);
                        else BattleManager.Instance.Entry();
                        break;
                    case "Shop":
                        ShopManager.Instance.Entry();
                        break;
                }
                _loadingSceneName = null;
                _asyncOperation.allowSceneActivation = false;
            };
        }

        public void LoadBattleScene(MapNode node, List<EntityData> entityDatas = null) {
            if (_globalUIManager == null) Init();
            if (entityDatas.Count > 0) this._entityDatas = entityDatas;
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

        public void LoadRecoverScene(MapNode node) {
            if (_globalUIManager == null) Init();
            _globalUIManager.stageAlertUI.Show(
                node,
                () => HandleRecoverNode(node)
            );
        }


        private void HandleRecoverNode(MapNode node) {
            int healAmount = 30;

            var players = PlayerStateManager.Instance.GetAllPlayer();

            foreach (var player in players) {
                int currentHp = player.hp;

                int maxHp = EntityFactory.GetHp(player.entityClass);

                int actualHealAmount = Mathf.Min(healAmount, maxHp - currentHp);

                if (actualHealAmount > 0) {
                    PlayerStateManager.Instance.ModifyHP(player.playerId, actualHealAmount);
                    Debug.Log($"{player.playerName} 回復了 {actualHealAmount} 點血量 ({currentHp} -> {currentHp + actualHealAmount})");
                }
                else {
                    Debug.Log($"{player.playerName} 血量已滿 ({currentHp}/{maxHp})");
                }
            }

            node.Resolve();
        }
    }
}
