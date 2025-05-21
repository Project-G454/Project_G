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
            List<MapNode> nodes = MapGenerator.Generate(7, 15);
            foreach (MapNode node in nodes) {
                GameObject newNodeObj = Instantiate(nodePrefab, nodeParent);
                MapNode newNode = newNodeObj.GetComponent<MapNode>();
                newNode.Init(node);
                newNodeObj.transform.localPosition = newNode.position * 2;
            }
            DrawLines(nodes);
        }

        public void DrawLines(List<MapNode> nodes) {
            foreach (MapNode node in nodes) {
                foreach (MapNode next in node.connectedNodes) {
                    Debug.DrawLine(node.position * 2, next.position * 2, Color.red, 60f);
                }
            }
        }

        void IManager.Init() {
            
        }
    }
}
