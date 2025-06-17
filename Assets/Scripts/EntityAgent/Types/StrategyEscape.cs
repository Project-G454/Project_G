using System.Collections.Generic;
using System.Linq;
using Agents.Data;
using Agents.Handlers;
using Agents.Helpers;
using Core.Entities;
using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyEscape: AgentStrategy {
        public override bool Execute(EntityAgent agent) {
            base.Execute(agent);
            agent.endMoving = false;
            Entity player = _FindNearestPlayer();
            if (player == null) return false;
            Vector2 targetPos = _FindBestEscapePosition(5);
            Vector2 bestPos = base._FindBestPosition(targetPos, 5);
            if (!CanMoveTo(bestPos)) {
                _agent.canMove = false;
                agent.endMoving = true;
                return false;
            }
            MapManager.Instance.MoveTo(bestPos, () => {
                agent.endMoving = true;
                Debug.Log("Agent moving end");
            });
            if (bestPos == (Vector2)agent.transform.position) {
                _agent.canMove = false;
                return false;
            }
            return true;
        }

        // --- helper functions ---

        private Vector2 _FindBestEscapePosition(int detectDistance) {
            Vector2 agentPos = _agent.transform.position;
            base.DebugDrawPoint(agentPos, Color.blue, 0.3f, 1f);

            Queue<Vector2> q = new();
            HashSet<Vector2> visited = new();

            // 初始化
            q.Enqueue(agentPos);
            visited.Add(agentPos);

            Vector2 res = agentPos;
            float maxScore = 0;

            while (q.Count > 0) {
                Vector2 curr = q.Dequeue();

                // 超出地圖範圍 -> 跳過
                if (!IsTile(curr)) {
                    Debug.LogWarning($"{curr} is not tile");
                    continue;
                }

                // 在可移動範圍內，且不為障礙物 -> 可視為目的地
                if (DistanceHelper.InRange(curr, agentPos, detectDistance) && _IsWalkable(curr)) {
                    base.DebugDrawPoint(curr, Color.red, 0.2f, 1f);
                    float currScore = calcScore(curr);

                    if (currScore > maxScore && DistanceHelper.ManhattanDistance(agentPos, curr) <= detectDistance) {
                        res = curr;
                        maxScore = currScore;
                    }
                }

                // 搜尋四個方向
                Vector2[] directions = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
                foreach (Vector2 dir in directions) {
                    Vector2 next = curr + dir;
                    if (!visited.Contains(next)) {
                        visited.Add(next);
                        q.Enqueue(next);
                    }
                }
            }

            return res;
        }

        private float calcScore(Vector2 pos) {
            float score = 0;
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            foreach (Entity player in players) {
                score += Vector2.Distance(player.position, pos);
            }
            return score;
        }
    }
}
