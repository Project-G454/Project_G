
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap.Models {
    [CreateAssetMenu(fileName = "New Node", menuName = "World Map/Node")]
    public class MapNodeData: ScriptableObject {
        public string nodeName;
        public NodeType nodeType;
        public Sprite icon;
        public string description;
    }
}
