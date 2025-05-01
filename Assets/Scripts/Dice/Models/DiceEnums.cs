using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dices.Data {
    [CreateAssetMenu(fileName = "D6", menuName = "Dices/D6")]
    public class DiceD6Rotation: ScriptableObject {
        public List<DiceRotationEntry> relativeRotation;
    }

    [Serializable]
    public class DiceRotationEntry {
        public int from;
        public List<DiceRotationInfo> rotation;
    }

    [Serializable]
    public class DiceRotationInfo {
        public int to;
        public Vector2 rotation;
    }
}
