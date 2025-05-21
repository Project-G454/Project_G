using System;
using System.Collections.Generic;
using System.Linq;
using Core.Loaders.WorldMap;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WorldMap.Models;

namespace WorldMap {
    public class MapGenerator {
        private static readonly Dictionary<NodeType, float> nodeSpawnChances = new Dictionary<NodeType, float> {
            {NodeType.Battle, 0.04f},
            {NodeType.Shop, 0.02f},
            {NodeType.Recover, 0.02f}
        };

        public static List<MapNode> Generate(int width, int height) {
            var grid = GenerateGrid(width, height, 0.7f);
            Vector2Int startPos = PickStartNode(grid);
            var map = GeneratePath(startPos, grid);
            var typedMap = AssignTypes(grid);
            List<MapNode> nodes = CombineMapInfo(startPos, grid, map, typedMap);
            return nodes;
        }

        public static List<List<bool>> GenerateGrid(int width, int height, float chance = 0.5f) {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be > 0");

            List<List<bool>> map = new List<List<bool>>(height);
            for (int i = 0; i < height; i++) {
                List<bool> row = new List<bool>(width);
                for (int j = 0; j < width; j++) {
                    float rng = UnityEngine.Random.Range(0f, 1f);
                    row.Add(rng < chance);
                }
                map.Add(row);
            }
            return map;
        }

        public static Vector2Int PickStartNode(List<List<bool>> grid) {
            List<bool> firstRow = grid[0];
            List<int> nodes = new();
            for (int i = 0; i < firstRow.Count; i++) {
                if (firstRow[i]) nodes.Add(i);
            }

            if (nodes.Count == 0)
                throw new Exception("Length of first row must be > 0");

            int rng = UnityEngine.Random.Range(0, nodes.Count);
            return new Vector2Int(nodes[rng], 0);
        }

        public static List<List<List<Vector2Int>>> GeneratePath(Vector2Int startPos, List<List<bool>> grid) {
            int height = grid.Count;
            int width = grid[0].Count;

            grid[startPos.y][startPos.x] = true;

            // 生成一個 2D-Array 作為地圖 (每個地圖格會儲存下一排的多個格子座標，表示連接到該格 (單向))
            List<List<List<Vector2Int>>> map = new List<List<List<Vector2Int>>>(height);
            for (int i = 0; i < height; i++) {
                // 初始化每一橫排
                List<List<Vector2Int>> row = new List<List<Vector2Int>>(width);
                for (int j = 0; j < width; j++) {
                    // 初始化每一格
                    List<Vector2Int> next = FindNextNode(new Vector2Int(j, i), grid);
                    if (j > 0) next = RemoveCrossedPath(row[j - 1], next);
                    row.Add(next);
                }
                map.Add(row);
            }

            return map;
        }

        public static List<Vector2Int> FindNextNode(Vector2Int currPos, List<List<bool>> grid, float connectChance = 0.5f) {
            int height = grid.Count;
            int width = grid[0].Count;

            List<Vector2Int> next = new List<Vector2Int>();

            // 如果這格不是地圖格 -> 跳過
            if (!grid[currPos.y][currPos.x] || currPos.y + 1 == height) {
                return next;
            }

            // 當前格往 左/中/右 嘗試連接
            for (int k = Math.Max(currPos.x - 1, 0); k <= Math.Min(currPos.x + 1, width - 1); k++) {
                // 如果這個方向的下一格不是地圖格 -> 跳過
                if (!grid[currPos.y + 1][k]) continue;

                // 隨機決定是否連接
                float rng = UnityEngine.Random.Range(0f, 1f);
                if (rng > connectChance) continue;
                next.Add(new Vector2Int(k, currPos.y + 1));
            }
            return next;
        }

        public static List<List<NodeType>> AssignTypes(List<List<bool>> grid) {
            int height = grid.Count;
            int width = grid[0].Count;
            List<List<NodeType>> typedMap = new List<List<NodeType>>(height);
            for (int i = 0; i < height; i++) {
                List<NodeType> row = new List<NodeType>(width);
                for (int j = 0; j < width; j++) {
                    if (!grid[i][j]) row.Add(NodeType.Unset);
                    else row.Add(GetRandomNode());
                }
                typedMap.Add(row);
            }
            return typedMap;
        }

        public static List<MapNode> CombineMapInfo(
            Vector2Int startPos,
            List<List<bool>> grid,
            List<List<List<Vector2Int>>> path,
            List<List<NodeType>> types
        ) {
            int height = grid.Count;
            int width = grid[0].Count;
            var map = new Dictionary<Vector2Int, MapNode>();
            grid[startPos.y][startPos.x] = true;

            // 檢查並重新連接死路
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (!grid[i][j]) continue;

                    if (path[i][j].Count == 0 && i + 1 < height) {
                        var next = FindNextNode(new Vector2Int(j, i), grid, 1f);
                        if (j > 0) next = RemoveCrossedPath(path[i][j - 1], next);
                        path[i][j] = next;

                        if (next.Count > 0) continue;

                        // 標記道路節點
                        grid[i + 1][j] = true;
                        path[i][j].Add(new Vector2Int(j, i+1));
                        types[i][j] = NodeType.Road;

                        // 嘗試連接道路節點到他周圍的格子
                        next = FindNextNode(new Vector2Int(j, i+1), grid);
                        if (j > 0) next = RemoveCrossedPath(path[i+1][j - 1], next);
                        path[i + 1][j] = next;
                    }

                }
            }

            // 建立節點
            int id = 0;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (!grid[i][j]) continue;
                    MapNodeData data = MapNodeFactory.GetNodeData(types[i][j]);
                    Vector2Int pos = new Vector2Int(j, i);
                    if (pos == startPos) data = WorldMapLoader.LoadForkNode();
                    MapNode node = new MapNode(data, id++, pos);
                    map.Add(pos, node);
                }
            }

            // 連接節點
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (!grid[i][j]) continue;

                    Vector2Int pos = new Vector2Int(j, i);
                    MapNode node = map.GetValueOrDefault(pos);
                    List<Vector2Int> connected = path[i][j];
                    foreach (var connectPos in connected) {
                        MapNode nextNode = map.GetValueOrDefault(connectPos);
                        node.Connect(nextNode);
                    }
                }
            }

            Queue<MapNode> q = new();
            q.Enqueue(map.GetValueOrDefault(startPos));
            List<MapNode> connectedNodes = new();
            while (q.Count > 0) {
                MapNode curr = q.Dequeue();
                connectedNodes.Add(curr);
                foreach (MapNode node in curr.connectedNodes) {
                    q.Enqueue(node);
                }
            }
            return connectedNodes;
        }

        public static List<Vector2Int> RemoveCrossedPath(List<Vector2Int> a, List<Vector2Int> b) {
            List<Vector2Int> res = new();
            int maxX = -1;
            foreach (Vector2Int pos in a) {
                maxX = Math.Max(pos.x, maxX);
            }
            foreach (Vector2Int pos in b) {
                if (pos.x > maxX) res.Add(pos);
            }
            return res;
        }

        private static NodeType GetRandomNode() {
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
