using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Map.Generators {
    // 隨機漫步地圖生成器實現
    public class RandomWalkGenerator : IMapGenerator {

        private readonly RandomWalkGeneratorData _data;
        
        public RandomWalkGenerator(RandomWalkGeneratorData data) {
            _data = data;
        }
        
        public HashSet<Vector2Int> GenerateMap(int width, int height, Vector2Int startPosition, int boundaryOffset) {
            var floorPositions = RunRandomWalk(startPosition);
            
            // 確保房間在網格邊界內
            floorPositions = new HashSet<Vector2Int>(floorPositions.Where(
                pos => pos.x >= boundaryOffset && 
                      pos.x < width - boundaryOffset && 
                      pos.y >= boundaryOffset && 
                      pos.y < height - boundaryOffset));
            
            return floorPositions;
        }
        
        private HashSet<Vector2Int> RunRandomWalk(Vector2Int startPosition) {
            var currentPosition = startPosition;
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
            floorPositions.Add(currentPosition);
            
            for (int i = 0; i < _data.iterations; i++) {
                var path = SimpleRandomWalk(currentPosition, _data.walkLength);
                floorPositions.UnionWith(path);
                
                if (floorPositions.Count > 0) {
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                }
            }
            
            return floorPositions;
        }
        
        private HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength) {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPosition);
            var previousPosition = startPosition;
            
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),  // 上
                new Vector2Int(1, 0),  // 右
                new Vector2Int(0, -1), // 下
                new Vector2Int(-1, 0)  // 左
            };
            
            for (int i = 0; i < walkLength; i++) {
                var direction = directions[Random.Range(0, directions.Count)];
                var newPosition = previousPosition + direction;
                path.Add(newPosition);
                previousPosition = newPosition;
            }
            
            return path;
        }
    }
}
