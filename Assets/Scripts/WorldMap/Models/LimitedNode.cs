using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace WorldMap.Models {
    public class LimitedNode {
        public Vector2Int position;
        public HashSet<LimitedNode> connections;
        public NodeType type;

        public LimitedNode(int x, int y) {
            Vector2Int position = new Vector2Int(x, y);
            this.position = position;
            this.type = NodeType.Unset;
            this.connections = new();
        }

        public void Connect(LimitedNode next) {
            connections.Add(next);
        }

        public void AssignType(NodeType type) {
            this.type = type;
        }
    }
}
