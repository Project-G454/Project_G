using Core.Managers;
using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI {
    class SuicideBtn: MonoBehaviour {
        public void Suicide() {
            EventSystem.current.SetSelectedGameObject(null);
            BattleManager battleManager = BattleManager.Instance;
            if (battleManager == null || battleManager.currentEntity == null) return;
            battleManager.currentEntity.TakeDamage(battleManager.currentEntity.currentHp);
            CardManager.Instance.EndTurn();
        }
    }
}
