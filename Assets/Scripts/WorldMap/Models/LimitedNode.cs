using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace WorldMap.Models {
    public class LimitedNode {
        public Vector2Int position;
        public HashSet<LimitedNode> connections;
        public NodeType type;
        public HashSet<LimitedNode> prevConnections;

        public LimitedNode(int x, int y) {
            this.position = new Vector2Int(x, y);
            this.type = NodeType.Unset;
            this.connections = new();
            this.prevConnections = new();
        }

        public void Connect(LimitedNode next) {
            next.prevConnections.Add(this);
            connections.Add(next);
        }

        public void Disconnect(LimitedNode target) {
            this.connections.Remove(target);
            target.prevConnections.Remove(this);
        }

        public void AssignType(NodeType type) {
            this.type = type;
        }

        public override bool Equals(object obj) {
            if (obj is LimitedNode other) {
                return this.position == other.position;
            }
            return false;
        }

        public override int GetHashCode() {
            return position.GetHashCode();
        }
    }
}
