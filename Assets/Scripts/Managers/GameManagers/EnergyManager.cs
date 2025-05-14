using UnityEngine;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Core.Managers.Energy {
    /// <summary>
    /// 管理玩家能量邏輯。
    /// </summary>
    public class EnergyManager: MonoBehaviour, IManager {
        private int _energy;
        
        public int energy {
            get { return _energy; }
            set {
                if (value < 0)
                    _energy = 0;
                else
                    _energy = value;
                
                OnEnergyChanged?.Invoke(energy, maxEnergy); // 通知 UI
            }
        }

        public event Action<int, int> OnEnergyChanged;
        public int maxEnergy = 3;
        public int defaultRecover = 3;
        public int recover = 3;
        public List<IEnergySource> energySources = new();

        public void Init() { }

        /// <summary>
        /// 戰鬥開始，初始化能量。
        /// </summary>
        public void InitializeEnergy() {
            UpdateEnergyRecover();
            RecoverEnergy();
        }

        /// <summary>
        /// 回合開始，恢復能量。
        /// </summary>
        public void RecoverEnergy() {
            if (recover < 0) return;
            energy = Math.Min(maxEnergy, energy + recover);
        }

        public void UpdateEnergyRecover() {
            int bonus = energySources.Sum(src => src.GetEnergyModifier());
            recover = defaultRecover + bonus;
        }

        public void RegisterSource(IEnergySource source) {
            energySources.Add(source);
        }

        public void UnregisterSource(IEnergySource source) {
            energySources.Remove(source);
        }

        public void Add(int amount) {
            energy += amount;
        }

        public void Remove(int amount) {
            energy -= amount;
        }
    }
}
