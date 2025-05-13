using TMPro;
using UnityEngine;
using Core.Managers.Energy;

namespace Core.UI {
    public class EnergyUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI energyText;

        public void Bind(EnergyManager energyManager) {
            energyManager.OnEnergyChanged += UpdateText;
        }

        public void UnBind(EnergyManager energyManager) {
            energyManager.OnEnergyChanged -= UpdateText;
        }

        private void UpdateText(int energy, int maxEnergy) {
            energyText.text = $"{energy}/{maxEnergy}";
        }
    }
}
