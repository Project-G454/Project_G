using TMPro;
using UnityEngine;

namespace Descriptions {
    class DescriptionView: MonoBehaviour {
        public TMP_Text title;
        public TMP_Text description;

        public void SetView(Description descData) {
            title.text = descData.title;
            description.text = descData.description;
        }
    }
}
