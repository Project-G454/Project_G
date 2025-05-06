using Core.Interfaces;
using TMPro;
using UnityEngine;


namespace Core.Managers.Energy {
    public class EnergyUIManager: MonoBehaviour, IManager
    {
        public static EnergyUIManager Instance;
        public TextMeshProUGUI energyText;


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

        public void Bind(EnergyManager energyManager)
        {
            energyManager.OnEnergyChanged += UpdateText;
        }

        public void UnBind(EnergyManager energyManager)
        {
            energyManager.OnEnergyChanged -= UpdateText;
        }


        private void UpdateText(int energy, int maxEnergy)
        {
            energyText.text = $"{energy}/{maxEnergy}";
        }
    }
}
