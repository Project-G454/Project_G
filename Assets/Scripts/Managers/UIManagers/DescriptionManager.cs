using Core.Interfaces;
using UnityEngine;

namespace Core.Managers {
    public class DescriptionManager: MonoBehaviour, IManager {
        public static DescriptionManager Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        public void Init() {
            
        }
    }
}
