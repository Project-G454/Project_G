using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Loaders.WorldMap;
using Mono.Cecil;
using UnityEngine;
using WorldMap;
using WorldMap.Models;

namespace Core.Managers.WorldMap {
    class WorldMapManager: MonoBehaviour, IManager {
        public static WorldMapManager Instance { get; private set; }
        public GameObject nodePrefab;
        public Transform nodeParent;
        public bool isInit = false;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() {
            if (isInit) return;
            // GenerateNode();
            GenerateMap();
            isInit = true;
        }

        public void GenerateMap() {
            HashSet<LimitedNode> nodes = MapGenerator.Generate(7, 15);
            HashSet<MapNode> map = new HashSet<MapNode>();
            Dictionary<LimitedNode, MapNode> relation = new Dictionary<LimitedNode, MapNode>();

            int id = 0;
            foreach (LimitedNode node in nodes) {
                GameObject newNodeObj = Instantiate(nodePrefab, nodeParent);
                MapNodeData data = MapNodeFactory.GetNodeData(node.type);
                MapNode newNode = newNodeObj.GetComponent<MapNode>();
                newNode.Init(id++, data, node);
                newNodeObj.transform.localPosition = newNode.position * 2;
                map.Add(newNode);
                relation.Add(node, newNode);
            }

            foreach ((var k, var v) in relation) {
                foreach (var c in k.connections) {
                    if (relation.TryGetValue(c, out var node)) {
                        v.Connect(node);
                    }
                }
            }

            DrawLines(map);
        }

        public void DrawLines(HashSet<MapNode> map) {
            foreach (MapNode node in map) {
                foreach (MapNode next in node.connectedNodes) {
                    Debug.DrawLine(node.position * 2, next.position * 2, Color.red, 60f);
                }
            }
        }

        private void _Copy_Connections() {

        }

        void IManager.Init() {
            
        }
    }
}
