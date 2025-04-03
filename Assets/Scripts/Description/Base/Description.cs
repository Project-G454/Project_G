using UnityEngine;

namespace Descriptions {
    class Description: MonoBehaviour {
        public string title;
        public string description;

        public Description(string title, string description) {
            this.title = title;
            this.description = description;
        }
    }
}
