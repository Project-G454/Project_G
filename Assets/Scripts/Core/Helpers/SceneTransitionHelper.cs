using System.Collections.Generic;
using Core.Managers;
using Core.Managers.WorldMap;
using UnityEngine.SceneManagement;

namespace Core.Helpers {
    public static class SceneTransitionHelper {
        public static void LoadBattleScene() {
            SceneManager.LoadSceneAsync("SceneCiel").completed += _ => {
                BattleManager.Instance.Entry(); // 呼叫 Entry 初始化場景邏輯
            };
        }

        public static void LoadBattleRewardsScene() {
            SceneManager.LoadSceneAsync("BattleRewards").completed += _ => {
                BattleRewardsManager.Instance.Entry(); // 呼叫 Entry 初始化場景邏輯
            };
        }

        public static void LoadWorldMapScene() {
            SceneManager.LoadSceneAsync("WorldMap").completed += _ => {
                WorldMapManager.Instance.Entry(); // 呼叫 Entry 初始化場景邏輯
            };
        }
    }
}
