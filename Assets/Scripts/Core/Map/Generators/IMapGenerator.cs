using System.Collections.Generic;
using UnityEngine;

namespace Core.Map.Generators {
    // 地圖生成器接口
    public interface IMapGenerator {
        HashSet<Vector2Int> GenerateMap(int width, int height, Vector2Int startPosition, int boundaryOffset);
    }
}
