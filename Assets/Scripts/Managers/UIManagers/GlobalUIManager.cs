using Core.Interfaces;
using UnityEngine;
using Core.UI;

namespace Core.Managers {
    public class GlobalUIManager: MonoBehaviour, IManager
    {
        public static GlobalUIManager Instance;

        public EnergyUI energyUI;
        public FreeStepUI freestepUI;


        private void Awake()
        {   
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
