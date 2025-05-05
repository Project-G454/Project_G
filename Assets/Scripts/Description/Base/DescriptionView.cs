using TMPro;
using UnityEngine;

namespace Descriptions {
    public class DescriptionView: MonoBehaviour {
        public TMP_Text title;
        public TMP_Text description;

        public void SetView(Description descData) {
            title.text = descData.title;
            description.text = descData.description;
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}
