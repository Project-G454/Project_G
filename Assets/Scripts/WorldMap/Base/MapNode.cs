using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using WorldMap.Models;

namespace WorldMap {
    public class MapNode: MonoBehaviour {
        public MapNodeData data;
        public int id;
        public Vector2 position;
        public List<MapNode> connectedNodes = new();
        public MapNodeView view;
        public bool isLocked = true;
        public bool isVisited = false;

        public MapNode(MapNodeData data, int id, Vector2 pos) {
            this.data = data;
            this.id = id;
            this.position = pos;
        }

        public void Init(int id, MapNodeData data, LimitedNode node) {
            this.data = data;
            this.id = id;
            this.position = node.position;
            this.isLocked = node.position.y > 0;
            this.isVisited = node.position.y == 0;
            this.view = GetComponent<MapNodeView>();
            if (data != null) view.SetView(data.icon);
        }

        public void Connect(MapNode node) {
            this.connectedNodes.Add(node);
        }

        public void OnMouseUp() {
            EntryManagerDispatcher.Enter(this.data.nodeType);
        }
    }
}
