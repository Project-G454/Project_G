using System;
using System.Collections.Generic;
using System.Linq;
using Core.Loaders.WorldMap;
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

            var typedMap = AssignTypes(map);
            var nodeMap = RemoveCrossedPath(typedMap, height);
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
            for (int i = 0; i < height - 1; i++) {
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

            // 先加入舊節點
            foreach (var node in oldMap) {
                positionMap[node.position] = node;
            }

            // 加入新節點，如果位置重複，就合併連線
            foreach (var node in newMap) {
                if (positionMap.TryGetValue(node.position, out var existing)) {
                    // 合併連線
                    foreach (var connected in node.connections) {
                        if (positionMap.TryGetValue(connected.position, out var connectedOld)) {
                            existing.Connect(connectedOld);
                        } else {
                            existing.Connect(connected); // 可能是新節點
                        }
                    }
                }
                else positionMap[node.position] = node;
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

        public static HashSet<LimitedNode> RemoveCrossedPath(HashSet<LimitedNode> map, int height) {
            HashSet<LimitedNode> heads = new();
            HashSet<LimitedNode> temp = new();
            foreach (LimitedNode node in map) {
                if (node.position.y == 0) temp.Add(node);
            }

            Dictionary<int, List<int>> visitedX = new();
            HashSet<LimitedNode> crossedNodes = new();
            for (int i = 0; i < height - 1; i++) {
                heads.Clear();
                crossedNodes.Clear();
                heads.UnionWith(temp);
                temp.Clear();
                visitedX.Clear();
                foreach (LimitedNode head in heads) {
                    crossedNodes.Clear();
                    foreach (LimitedNode node in head.connections) {
                        List<int> fromX = new();
                        if (!visitedX.TryGetValue(node.position.x, out fromX)) {
                            // 如果下一排的該節點未被檢查過
                            visitedX.Add(node.position.x, new List<int> { head.position.x });
                            temp.Add(node);
                            continue;
                        }

                        // 如果已經確定下一排的節點至少有一條 path
                        if (head.position.x > fromX.Min() && head.position.x > node.position.x)
                            crossedNodes.Add(node); // 與左邊的節點交叉
                        else if (head.position.x < fromX.Max() && head.position.x > node.position.x)
                            crossedNodes.Add(node); // 與右邊的節點交叉
                        else {
                            fromX.Add(head.position.x);
                            visitedX[node.position.x] = fromX;
                        }
                    }
                    foreach (LimitedNode node in crossedNodes) {
                        head.connections.Remove(node);
                    }
                }
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
