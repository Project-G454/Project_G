using Core.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI {
    class PlayerStatusView: MonoBehaviour {
        public Image playerIcon;
        public TMP_Text healthAmount;
        public TMP_Text goldAmount;
        public TMP_Text deckAmount;

        public void Set(GamePlayerState state) {
            playerIcon.sprite = state.avatar;
            healthAmount.text = state.currentHp.ToString();
            goldAmount.text = state.gold.ToString();
            deckAmount.text = state.deck.Count.ToString();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerIcon.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(healthAmount.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(goldAmount.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(deckAmount.rectTransform);
        }
    }
}
