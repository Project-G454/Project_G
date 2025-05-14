using Core.Interfaces;
using UnityEngine;

namespace Core.Helpers {
    public class ManagerHelper {
        public static T RequireManager<T>(T Instance) where T: class, IManager {
            if (Instance == null) {
                Debug.LogError(typeof(T).Name + " 載入失敗");
            }
            else {
                Instance.Init();
            }
            return Instance;
        }
    }
}
