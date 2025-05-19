using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cards {
    public class CardEffectDisplay: MonoBehaviour {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text effectText;

        /// <summary> 設定圖示與數值 </summary>
        public void SetDisplay(Sprite icon, int round) {
            if (iconImage != null) iconImage.sprite = icon;
            if (effectText != null) effectText.text = round.ToString();
        }
    }
}
