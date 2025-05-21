using Core.Loaders.WorldMap;
using WorldMap.Models;

namespace WorldMap {
    public class MapNodeFactory {
        public static MapNodeData GetNodeData(NodeType type) {
            MapNodeData data = type switch {
                NodeType.Shop => WorldMapLoader.LoadShopNode(),
                NodeType.Battle => WorldMapLoader.LoadBattleNode(),
                NodeType.Recover => WorldMapLoader.LoadRecoverNode(),
                NodeType.Fork => WorldMapLoader.LoadForkNode(),
                NodeType.Road => WorldMapLoader.LoadRoadNode(),
                _ => WorldMapLoader.LoadRoadNode()
            };
            return data;
        }
    }
}
