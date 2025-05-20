using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Handlers {
    class ForceUpdateFitter: MonoBehaviour {
        public RectTransform background;
        public RectTransform border;

        public void Fit() {
            Canvas.ForceUpdateCanvases(); // ✅ 只在更新 UI 內容後呼叫一次
            LayoutRebuilder.ForceRebuildLayoutImmediate(background);
            LayoutRebuilder.ForceRebuildLayoutImmediate(border);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
