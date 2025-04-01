using UnityEngine;
using Core.Interfaces;

namespace Core.Managers.Energy {
    /// <summary>
    /// 管理玩家能量邏輯。
    /// </summary>
    public class EnergyManager: MonoBehaviour, IManager {
        private int _energy;

        public int energy
        {
            get { return _energy; }
            set
            {
                if (value < 0)
                    _energy = 0;
                else if (value > maxEnergy)
                    _energy = maxEnergy;
                else
                    _energy = value;
            }
        }

        public int maxEnergy = 10;
        public int recover = 3;
        public int firstRoundExtra = 0;
        
        public void Init() {}

        /// <summary>
        /// 戰鬥開始，初始化能量。
        /// </summary>
        public void InitializeEnergy() {
            energy = firstRoundExtra;
        }

        /// <summary>
        /// 回合開始，恢復能量。
        /// </summary>
        public void RecoverEnergy() {
            energy += recover;
        }

        public void Add(int number) {
            energy -= number;
        }

        public void Remove(int number) {
            energy += number;
        }
    }
}
