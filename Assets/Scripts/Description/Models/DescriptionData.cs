using UnityEngine;

namespace Descriptions.Models {
    [CreateAssetMenu(fileName = "New Describe", menuName = "Description Data")]
    class DescriptionData: ScriptableObject {
        public string title;
        public string description;
    }
}
