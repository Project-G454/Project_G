using UnityEngine;

namespace Descriptions.Models {
    [CreateAssetMenu(fileName = "New Describe", menuName = "Description Data")]
    public class DescriptionData: ScriptableObject {
        public int id;
        public string title;
        public string description;
    }
}
