using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Loaders.WorldMap;
using Mono.Cecil;
using UnityEngine;
using WorldMap;
using WorldMap.Models;

namespace Core.Managers.WorldMap {
    class WorldMapManager: MonoBehaviour, IManager, IEntryManager {
        public static WorldMapManager Instance { get; private set; }
        public GameObject nodePrefab;
        public Transform nodeParent;
        public GameObject linePrefab;
        public Transform lineParent;
        public bool isInit = false;
        public HashSet<LimitedNode> nodes;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Reset() {}

        void Start() {
            Entry();
        }

        public void Entry() {
            if (!isInit) {
                nodes = MapGenerator.Generate(7, 15, 6);
            }
            HashSet<MapNode> map = GenerateMap(nodes);
            DrawMap(map);
            isInit = true;
        }

        public HashSet<MapNode> GenerateMap(HashSet<LimitedNode> nodes) {
            
            HashSet<MapNode> map = new HashSet<MapNode>();
            Dictionary<LimitedNode, MapNode> relation = new Dictionary<LimitedNode, MapNode>();

            int id = 0;
            foreach (LimitedNode node in nodes) {
                GameObject newNodeObj = Instantiate(nodePrefab, nodeParent);
                MapNodeData data = MapNodeFactory.GetNodeData(node.type);
                MapNode newNode = newNodeObj.GetComponent<MapNode>();
                newNode.Init(id++, data, node);
                newNodeObj.transform.position = newNode.position * 2;
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

            return map;
        }

        public void DrawLines(HashSet<MapNode> map) {
            foreach (MapNode node in map) {
                foreach (MapNode next in node.connectedNodes) {
                    Debug.DrawLine(node.position * 2 + Vector2.left * 9, next.position * 2 + Vector2.left * 9, Color.red, 60f);
                }
            }
        }

        public void DrawMap(HashSet<MapNode> map) {
            foreach (MapNode node in map) {
                foreach (MapNode next in node.connectedNodes) {
                    CreateLine(node.position * 2, next.position * 2);
                }
            }
        }

        public void CreateLine(Vector2 from, Vector2 to) {
            GameObject newLine = Instantiate(linePrefab, lineParent);
            LineRenderer lr = newLine.GetComponent<LineRenderer>();
            if (lr != null) {
                lr.positionCount = 2;
                lr.SetPosition(0, from);
                lr.SetPosition(1, to);
            }
        }

        void IManager.Init() {
            
        }
    }
}
