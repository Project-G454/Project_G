using UnityEngine;

namespace Core.Map.Generators {
    // Inspector中設置參數
    [System.Serializable]
    public class RandomWalkGeneratorData {
        public int iterations = 10;
        public int walkLength = 10;
    }
}
