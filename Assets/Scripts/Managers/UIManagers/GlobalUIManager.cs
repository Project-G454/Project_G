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
        public StageAlertUI stageAlertUI;
        public ConfirmAlertUI confirmAlertUI;

        private void Awake() {
            Instance = this;
        }

        public void Reset() {}

        public void Init() {}
    }
}
