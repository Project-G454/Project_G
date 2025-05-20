using UnityEngine;
using UnityEngine.UI;

namespace Core.UI {
    public class FreeStepUI : MonoBehaviour {
        [SerializeField] private Image _stepImage;

        /// <summary>
        /// 顯示或隱藏步數圖示
        /// </summary>
        public void SetVisible(bool isVisible) {
            _stepImage.enabled = isVisible;
        }
    }
}
