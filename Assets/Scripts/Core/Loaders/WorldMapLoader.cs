using UnityEngine;
using WorldMap.Models;

namespace Core.Loaders.WorldMap {
    public class WorldMapLoader {
        public static MapNodeData LoadBattleNode() {
            return Resources.Load<MapNodeData>("WorldMap/Battle");
        }

        public static MapNodeData LoadShopNode() {
            return Resources.Load<MapNodeData>("WorldMap/Shop");
        }

        public static MapNodeData LoadStartNode() {
            return Resources.Load<MapNodeData>("WorldMap/Start");
        }

        public static MapNodeData LoadRecoverNode() {
            return Resources.Load<MapNodeData>("WorldMap/Recover");
        }

        public static MapNodeData LoadRoadNode() {
            return Resources.Load<MapNodeData>("WorldMap/Road");
        }

        public static MapNodeData LoadBossNode() {
            return Resources.Load<MapNodeData>("WorldMap/Boss");
        }
    }
}
