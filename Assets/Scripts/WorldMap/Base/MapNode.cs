using System.Collections.Generic;
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

        public void CopyValues(MapNode node) {
            this.data = node.data;
            this.id = node.id;
            this.position = node.position;
            this.connectedNodes = node.connectedNodes;
            this.isLocked = node.isLocked;
            this.isVisited = node.isVisited;
        }

        public void Init(MapNode node) {
            CopyValues(node);
            this.view = GetComponent<MapNodeView>();
            if (data != null) view.SetView(data.icon);
        }

        public void Connect(MapNode node) {
            this.connectedNodes.Add(node);
        }

        public void OnMouseUp() {
            switch (data.nodeType) {
                case NodeType.Shop:
                    Debug.Log("Shop");
                    break;
                case NodeType.Battle:
                    Debug.Log("Battle");
                    break;
                case NodeType.Recover:
                    break;
            }
        }
    }
}
