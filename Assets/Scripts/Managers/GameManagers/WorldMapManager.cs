using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Loaders.WorldMap;
using Entities;
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
        public HashSet<MapNode> map;
        public int level = 1;
        public int currentStage = -1;
        public int currentNodeId = -1;
        public HashSet<int> resovedNodeIds = new();
        private List<int> _disabledNodeIds = new();
        private Vector3 _cameraPosition;
        private float _cameraZoom = -1;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() { }

        public void Reset() { }

        void Start() {
            Entry();
        }

        public void Entry() {
            if (!isInit) {
                nodes = MapGenerator.Generate(7, 15, 6);

                PlayerStateManager.Instance.AddPlayer(0, "Player1", EntityClasses.WARRIOR, 100);
                PlayerStateManager.Instance.AddPlayer(1, "Player2", EntityClasses.RANGER, 100);
                PlayerStateManager.Instance.AddPlayer(2, "Player3", EntityClasses.ROGUE, 100);
            }
            if (currentNodeId > 0) resovedNodeIds.Add(currentNodeId);
            LoadCameraState();
            map = GenerateMap(nodes);
            DrawMap(map);
            isInit = true;
        }

        public HashSet<MapNode> GenerateMap(HashSet<LimitedNode> nodes) {
            HashSet<MapNode> map = new HashSet<MapNode>();
            Dictionary<LimitedNode, MapNode> relation = new Dictionary<LimitedNode, MapNode>();
            List<MapNode> startNodes = new();
            List<MapNode> endNodes = new();
            List<MapNode> resovedNodes = new();
            List<MapNode> _disabledNodes = new();

            int id = 0;
            foreach (LimitedNode node in nodes) {
                MapNode newNode = _CreateNode(node, id++);
                map.Add(newNode);
                relation.Add(node, newNode);
                if (newNode.data.nodeType == NodeType.Start) startNodes.Add(newNode);
                if (newNode.isTail) endNodes.Add(newNode);
                if (resovedNodeIds.Contains(newNode.id)) resovedNodes.Add(newNode);
                if (
                    newNode.stage == currentStage &&
                    newNode.id != currentNodeId ||
                    _disabledNodeIds.Contains(newNode.id)
                ) {
                    Debug.Log($"ID: {newNode.id}, {newNode.stage} == {currentStage}, {newNode.id} != {currentNodeId}");
                    _disabledNodeIds.Add(newNode.id);
                    _disabledNodes.Add(newNode);
                }
            }

            float avgX = 0;
            float avgY = 0;
            foreach (var node in endNodes) {
                avgX += node.transform.position.x;
                avgY += node.transform.position.y;
            }
            Vector2 bossPos = new Vector2(avgX, avgY) / endNodes.Count;
            bossPos += Vector2.up * 7;

            MapNode bossNode = _CreateBossNode(id++, bossPos);
            map.Add(bossNode);

            foreach ((var k, var v) in relation) {
                foreach (var c in k.connections) {
                    if (relation.TryGetValue(c, out var node)) {
                        v.Connect(node);
                    }
                }
            }

            foreach (var node in startNodes) {
                node.Resolve();
            }

            foreach (var node in resovedNodes) {
                node.Resolve();
            }

            Debug.Log($"Disabling {_disabledNodes.Count} nodes");
            foreach (var node in _disabledNodes) {
                node.Lock();
            }

            foreach (var node in endNodes) {
                node.Connect(bossNode);
            }

            return map;
        }

        private MapNode _CreateNode(LimitedNode node, int id) {
            GameObject newNodeObj = Instantiate(nodePrefab, nodeParent);
            MapNodeData data = MapNodeFactory.GetNodeData(node.type);
            MapNode newNode = newNodeObj.GetComponent<MapNode>();
            newNode.Init(id, data, node);
            newNodeObj.transform.position = newNode.position * 5;
            return newNode;
        }

        private MapNode _CreateBossNode(int id, Vector2 pos) {
            GameObject newNodeObj = Instantiate(nodePrefab, nodeParent);
            MapNodeData data = MapNodeFactory.GetNodeData(NodeType.Boss);
            MapNode newNode = newNodeObj.GetComponent<MapNode>();
            newNode.Init(id, data, NodeType.Boss, pos);
            newNodeObj.transform.position = pos;
            newNode.transform.localScale = Vector3.one * 3;
            return newNode;
        }

        public void DrawMap(HashSet<MapNode> map) {
            foreach (MapNode node in map) {
                foreach (MapNode next in node.connectedNodes) {
                    GameObject nodeObj = node.gameObject;
                    GameObject nextObj = next.gameObject;
                    BoxCollider2D currCollider = nodeObj.GetComponent<BoxCollider2D>();
                    BoxCollider2D nextCollider = nextObj.GetComponent<BoxCollider2D>();
                    float currHeight = currCollider.bounds.extents.y * nodeObj.transform.localScale.y;
                    float nextHeight = nextCollider.bounds.extents.y * nextObj.transform.localScale.y;
                    Vector3 unit = (nextObj.transform.position - nodeObj.transform.position).normalized;
                    Vector3 from = nodeObj.transform.position + unit * (currHeight + 0.5f);
                    Vector3 to = nextObj.transform.position - unit * (nextHeight + 0.5f);
                    CreateLine(from, to);
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

        public void SaveCameraState() {
            _cameraPosition = Camera.main.transform.position;
            _cameraZoom = Camera.main.orthographicSize;
        }

        public void LoadCameraState() {
            if (_cameraPosition == null || _cameraZoom < 0) return;
            Camera.main.transform.position = _cameraPosition;
            Camera.main.orthographicSize = _cameraZoom;
        }
    }
}
