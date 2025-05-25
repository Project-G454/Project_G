using System;
using System.Collections.Generic;
using System.Linq;
using Core.Loaders.WorldMap;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using WorldMap.Models;

namespace WorldMap {
    public class MapGenerator {
        private static readonly Dictionary<NodeType, float> nodeSpawnChances = new Dictionary<NodeType, float> {
            {NodeType.Battle, 0.04f},
            {NodeType.Shop, 0.02f},
            {NodeType.Recover, 0.02f}
        };

        private static readonly Dictionary<int, NodeType> forceFloorsType = new Dictionary<int, NodeType> {
            {0, NodeType.Fork},
            {7, NodeType.Shop},
            {14, NodeType.Recover},
        };

        public static HashSet<LimitedNode> Generate(int width, int height, int genTimes = 6) {
            var grid = InitGrid(width, height);
            HashSet<LimitedNode> map = new();
            for (int i = 0; i < genTimes; i++) {
                Vector2Int startPos = PickStartNode(width);
                HashSet<LimitedNode> newMap = DisitionNode(grid, startPos);
                map = CombineMap(newMap, map);
            }

            map = RemoveNoHeadNode(map);

            var typedMap = AssignTypes(map);
            var nodeMap = RemoveCrossedPath(typedMap, height);
            nodeMap = RemoveNoHeadNode(nodeMap);
            return nodeMap;
        }

        public static List<List<bool>> InitGrid(int width, int height) {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be > 0");

            List<List<bool>> map = new List<List<bool>>(height);
            for (int i = 0; i < height; i++) {
                List<bool> row = new List<bool>(width);
                for (int j = 0; j < width; j++) {
                    row.Add(false);
                }
                map.Add(row);
            }
            return map;
        }

        public static HashSet<LimitedNode> DisitionNode(List<List<bool>> grid, Vector2Int startPos) {
            int height = grid.Count;
            int width = grid[0].Count;

            HashSet<LimitedNode> nodes = new();

            LimitedNode curr = new LimitedNode(startPos.x, startPos.y);
            LimitedNode prev;
            for (int i = 1; i < height; i++) {
                int pick = UnityEngine.Random.Range(
                    Math.Max(0, curr.position.x - 1),
                    Math.Min(curr.position.x + 2, width)
                );
                grid[i][pick] = true;
                prev = curr;
                curr = new LimitedNode(pick, i);
                prev.Connect(curr);
                nodes.Add(prev);
            }
            nodes.Add(curr);
            return nodes;
        }

        public static HashSet<LimitedNode> CombineMap(HashSet<LimitedNode> newMap, HashSet<LimitedNode> oldMap) {
            Dictionary<Vector2Int, LimitedNode> positionMap = new();

            foreach (var node in oldMap.Concat(newMap)) {
                if (!positionMap.TryGetValue(node.position, out var existing)) {
                    positionMap[node.position] = node;
                } else {
                    // 合併連線進 existing
                    foreach (var c in node.connections) {
                        var target = positionMap.TryGetValue(c.position, out var merged) ? merged : c;
                        existing.connections.Add(target);
                    }
                }
            }

            // 第二步：統一所有 connection 指向 positionMap 中的實體
            foreach (var node in positionMap.Values) {
                var newConnections = new HashSet<LimitedNode>();
                foreach (var c in node.connections) {
                    var unified = positionMap.TryGetValue(c.position, out var merged) ? merged : c;
                    newConnections.Add(unified);
                }
                node.connections = newConnections;
            }

            return positionMap.Values.ToHashSet();
        }



        public static Vector2Int PickStartNode(int width) {
            int rng = UnityEngine.Random.Range(0, width);
            return new Vector2Int(rng, 0);
        }

        public static HashSet<LimitedNode> AssignTypes(HashSet<LimitedNode> map) {
            foreach (var node in map) {
                if (forceFloorsType.TryGetValue(node.position.y, out NodeType type)) {
                    node.AssignType(type);
                }
                else node.AssignType(GetRandomType());
            }
            return map;
        }

        public static HashSet<LimitedNode> RemoveNoHeadNode(HashSet<LimitedNode> map) {
            HashSet<LimitedNode> removal = new();
            foreach (var node in map) {
                if (node.prevConntions.Count == 0 && node.position.y > 0) {
                    removal.Add(node);
                }

                if (node.position.y == 0 && node.connections.Count == 0) {
                    removal.Add(node);
                }
            }

            foreach (var node in removal) {
                map.Remove(node);
            }

            return map;
        }

        public static HashSet<LimitedNode> RemoveCrossedPath(HashSet<LimitedNode> map, int height) {
            Dictionary<Vector2Int, LimitedNode> heads = new();
            Dictionary<Vector2Int, LimitedNode> temp = new();
            foreach (LimitedNode node in map) {
                if (node.position.y == 0) temp.Add(node.position, node);
            }

            

            HashSet<LimitedNode> visiteds = new();
            HashSet<LimitedNode> crossedNodes = new();

            while (temp.Count > 0) {
                heads.Clear();
                foreach (var kvp in temp) {
                    heads[kvp.Key] = kvp.Value;
                }
                temp.Clear();
                Debug.Log($"Head x{heads.Count}");
                foreach (var head in heads.Values) {
                    Debug.Log(head.connections.Count);
                    foreach (var target in head.connections) {
                        if (!temp.Keys.Contains(target.position)) {
                            temp.Add(target.position, target);
                        }

                        // connect to left
                        if (
                            target.position.x < head.position.x &&
                            heads.TryGetValue(head.position + Vector2Int.left, out var _) &&
                            temp.TryGetValue(target.position + Vector2Int.right, out var _)
                        ) crossedNodes.Add(target);

                        // connect to right
                        if (
                            target.position.x > head.position.x &&
                            heads.TryGetValue(head.position + Vector2Int.right, out var _) &&
                            temp.TryGetValue(target.position + Vector2Int.left, out var _)
                        ) crossedNodes.Add(target);
                    }

                    foreach (var crossed in crossedNodes) {
                        head.Disconnect(crossed);
                    }
                    crossedNodes.Clear();
                }
                Debug.Log($"Column x{temp.Count}");
            }
            return map;
        }

        private static NodeType GetRandomType() {
            float totalWeight = nodeSpawnChances.Sum(e => e.Value);
            float rng = UnityEngine.Random.Range(0f, totalWeight);

            float sum = 0f;
            foreach (var pair in nodeSpawnChances) {
                sum += pair.Value;
                if (rng <= sum) return pair.Key;
            }

            return NodeType.Unset;
        }
    }
}
