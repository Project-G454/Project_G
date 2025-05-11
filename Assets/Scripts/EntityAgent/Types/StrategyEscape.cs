using System.Collections.Generic;
using System.Linq;
using Agents.Helpers;
using Core.Entities;
using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyEscape: AgentStrategy {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            Entity target = _FindNearestPlayer();
            if (target == null) return;
            Vector2 bestPos = _FindBestPosition(target.position, 5);
            MapManager.Instance.MoveTo(bestPos);
        }

        // --- helper functions ---

        private Vector2 _FindBestPosition(Vector2 targetPos, int maxDistance) {
            // BFS 找格子
            Vector2 agentPos = _agent.entity.position;
            Queue<Vector2> q = new();
            HashSet<Vector2> visited = new();
            Vector2 dv = agentPos - targetPos;
            q.Enqueue(agentPos + dv);

            Vector2 res = agentPos;
            float resDistance = 0;

            while (q.Count > 0) {
                Vector2 curr = q.Dequeue();
                visited.Add(curr);
                Debug.Log($"Finding {curr}");

                // 超出可移動範圍 -> 跳過
                if (!DistanceHelper.InRange(curr, agentPos, maxDistance)) {
                    continue;
                }

                // 在可移動範圍內，且不為障礙物 -> 可視為目的地
                if (_IsWalkable(curr)) {
                    // 以距離較遠的格子為目的地
                    float currDistance = Vector2.Distance(targetPos, curr);
                    if (currDistance > resDistance) { 
                        res = curr;
                        resDistance = currDistance;
                        // 如果已經是可走到的最大距離就直接 break
                        if (Vector2.Distance(agentPos, curr) == maxDistance) break;
                    }

                }

                // 四向搜尋
                if (!visited.Contains(curr+Vector2.up)) {
                    q.Enqueue(curr+Vector2.up);
                }
                if (!visited.Contains(curr+Vector2.left)) {
                    q.Enqueue(curr+Vector2.left);
                }
                if (!visited.Contains(curr+Vector2.down)) {
                    q.Enqueue(curr+Vector2.down);
                }
                if (!visited.Contains(curr+Vector2.right)) {
                    q.Enqueue(curr+Vector2.right);
                }
            }

            return res;
        }

        private bool _IsWalkable(Vector2 pos) {
            Tile tile = GridManager.Instance.GetTileAtPosition(pos);
            return tile != null && tile.Walkable;
        }
    }
}
