using Core.Interfaces;
using UnityEngine;
using Core.UI;
using System.Collections.Generic;
using Entities;
using Core.Entities;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using Entities.Categories;

namespace Core.Managers {
    public class GlobalUIManager: MonoBehaviour, IManager {
        public static GlobalUIManager Instance;
        public EnergyUI energyUI;
        public FreeStepUI freestepUI;
        public TurnPanelUI turnPanelUI;
        public CardActiveUI cardActiveUI;

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
