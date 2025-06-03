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
            {0, NodeType.Start},
            {1, NodeType.Shop},
            {2, NodeType.Shop},
            {7, NodeType.Shop},
            {14, NodeType.Recover},
        };

        public static HashSet<LimitedNode> Generate(int width, int height, int genTimes = 6) {
            var grid = _InitGrid(width, height);
            HashSet<LimitedNode> map = new();
            for (int i = 0; i < genTimes; i++) {
                Vector2Int startPos = _PickStartNode(width);
                HashSet<LimitedNode> newMap = _DisitionNode(grid, startPos);
                map = _CombineMap(newMap, map);
            }

            var typedMap = _AssignTypes(map);
            var nodeMap = _RemoveCrossedPath(typedMap);
            nodeMap = _RemoveNoHeadNode(nodeMap, height);
            return nodeMap;
        }

        private static List<List<bool>> _InitGrid(int width, int height) {
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

        private static HashSet<LimitedNode> _DisitionNode(List<List<bool>> grid, Vector2Int startPos) {
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
            curr.isTail = true;
            nodes.Add(curr);
            return nodes;
        }

        private static HashSet<LimitedNode> _CombineMap(HashSet<LimitedNode> newMap, HashSet<LimitedNode> oldMap) {
            Dictionary<Vector2Int, LimitedNode> positionMap = new();

            foreach (var node in oldMap.Concat(newMap)) {
                if (!positionMap.TryGetValue(node.position, out var existing)) {
                    positionMap[node.position] = node;
                    continue;
                }

                // 合併連線進 existing
                List<LimitedNode> disconnects = new();
                foreach (var c in node.connections) {
                    if (positionMap.TryGetValue(c.position, out var merged)) {
                        existing.Connect(merged);
                        disconnects.Add(merged);
                    }
                    else {
                        existing.Connect(c);
                        disconnects.Add(c);
                    }
                }

                foreach (var dis in disconnects) {
                    node.Disconnect(dis);
                }
            }

            // 第二步：統一所有 connection 指向 positionMap 中的實體
            foreach (var node in positionMap.Values) {
                var newConnections = new HashSet<LimitedNode>();
                foreach (var c in node.connections) {
                    var unified = positionMap.TryGetValue(c.position, out var merged) ? merged : c;
                    newConnections.Add(unified);
                }
                node.connections.Clear();
                foreach (var c in newConnections) {
                    node.Connect(c);
                }
            }

            return positionMap.Values.ToHashSet();
        }

        private static Vector2Int _PickStartNode(int width) {
            int rng = UnityEngine.Random.Range(0, width);
            return new Vector2Int(rng, 0);
        }

        private static HashSet<LimitedNode> _AssignTypes(HashSet<LimitedNode> map) {
            foreach (var node in map) {
                if (forceFloorsType.TryGetValue(node.position.y, out NodeType type)) {
                    node.AssignType(type);
                }
                else node.AssignType(_GetRandomType());
            }
            return map;
        }

        private static HashSet<LimitedNode> _RemoveNoHeadNode(HashSet<LimitedNode> map, int height) {
            HashSet<LimitedNode> bottomToTop = new();
            HashSet<LimitedNode> topToBottom = new();
            HashSet<LimitedNode> visited = new();
            Queue<LimitedNode> queue = new();

            // Find heads
            foreach (var node in map) {
                if (node.position.y == 0 && node.connections.Count > 0) {
                    visited.Add(node);
                    queue.Enqueue(node);
                }
            }

            // bottom to top
            while (queue.Count > 0) {
                LimitedNode curr = queue.Dequeue();
                foreach (var node in curr.connections) {
                    if (!visited.Contains(node)) {
                        visited.Add(node);
                        queue.Enqueue(node);
                    }
                }
                bottomToTop.Add(curr);
            }

            visited.Clear();
            queue.Clear();

            // Find tails
            foreach (var node in bottomToTop) {
                if (node.position.y == height - 1 && node.prevConnections.Count > 0) {
                    visited.Add(node);
                    queue.Enqueue(node);
                }
            }

            // top to bottom
            while (queue.Count > 0) {
                LimitedNode curr = queue.Dequeue();
                foreach (var node in curr.prevConnections) {
                    if (!visited.Contains(node)) {
                        visited.Add(node);
                        queue.Enqueue(node);
                    }
                }
                topToBottom.Add(curr);
            }

            // remove unlinked nodes
            HashSet<LimitedNode> removal = new();
            foreach (var node in map) {
                if (!bottomToTop.Contains(node) || !topToBottom.Contains(node)) {
                    removal.Add(node);
                }
            }

            List<List<LimitedNode>> removalPairs = new();
            foreach (var node in removal) {
                map.Remove(node);
                foreach (var prev in node.prevConnections) {
                    removalPairs.Add(new List<LimitedNode>{prev, node});
                }

                foreach (var next in node.connections) {
                    removalPairs.Add(new List<LimitedNode>{node, next});
                }
            }

            foreach (var pair in removalPairs) {
                pair[0].Disconnect(pair[1]);
            }

            return map;
        }

        private static HashSet<LimitedNode> _RemoveCrossedPath(HashSet<LimitedNode> map) {
            Dictionary<Vector2Int, LimitedNode> heads = new();
            Dictionary<Vector2Int, LimitedNode> temp = new();
            foreach (LimitedNode node in map) {
                if (node.position.y == 0) temp.Add(node.position, node);
            }

            HashSet<LimitedNode> crossedNodes = new();
            while (temp.Count > 0) {
                heads.Clear();
                foreach (var kvp in temp) {
                    heads[kvp.Key] = kvp.Value;
                }

                temp.Clear();
                foreach (var head in heads.Values) {
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
            }
            return map;
        }

        private static NodeType _GetRandomType() {
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
