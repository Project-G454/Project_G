using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Managers {
    [CreateAssetMenu(fileName = "New Terrain Style", menuName = "Game/Terrain Style")]
    public class TerrainStyleData: ScriptableObject {
        [Header("Terrain Information")]
        public string styleName = "Default Style";

        [Header("Y Offset")]
        public float obstacleYOffset = 0f;
        public float wallYOffset = 0f;

        [Header("Tile Setting")]
        public RuleTile floorTile;
        public RuleTile wallTile;
        public RuleTile decorationTile;
        public RuleTile obstacleTile;
        public RuleTile backgroundTile;

        [Header("Generation Parameters")]
        [Range(0f, 1f)]
        public float decorationProbability = 0.3f;
        [Range(0f, 1f)]
        public float obstacleProbability = 0.15f;

        [Header("Roguelike Settings")]
        [Tooltip("Weight for random selection (higher values are more likely to be selected)")]
        [Range(1f, 10f)]
        public float selectionWeight = 1f;

        public bool canBeRandomlySelected = true;
    }
}
