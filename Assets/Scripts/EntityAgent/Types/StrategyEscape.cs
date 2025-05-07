using System.Collections.Generic;
using System.Linq;
using Agents.Helpers;
using Core.Entities;
using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategy {
    class StrategyEscape: AgentDecision {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            Entity target = _FindNearestTarget();
            if (target == null) return;
            List<Vector2> bestPos = _FindBestPosition(target.position, 5);
            MapManager.Instance.MoveTo(bestPos.First());
        }

        // --- helper functions ---
        private Entity _FindNearestTarget() {
            List<Entity> entities = EntityManager.Instance.GetEntityList();
            float minDistance = 10;
            Entity target = null;
            foreach (Entity entity in entities) {
                if (_agent.entity.entityId == entity.entityId) continue;
                float distance = Vector2.Distance(_agent.entity.position, entity.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    target = entity;
                }
            }
            return target;
        }

        private List<Vector2> _FindBestPosition(Vector2 targetPos, int maxDistance) {
            Vector2 agentPos = _agent.entity.position;
            Queue<Vector2> q = new();
            HashSet<Vector2> visited = new();
            q.Enqueue(targetPos);
            List<Vector2> res = new();

            while (q.Count > 0) {
                Vector2 curr = q.Dequeue();
                visited.Add(curr);
                if (!IsBlocked(curr)) {
                    res.Add(curr);
                    continue;
                }

                if (!visited.Contains(curr+Vector2.up) && DistanceHelper.InRange(curr+Vector2.up, agentPos, maxDistance)) {
                    q.Enqueue(curr+Vector2.up);
                }
                if (!visited.Contains(curr+Vector2.left) && DistanceHelper.InRange(curr+Vector2.left, agentPos, maxDistance)) {
                    q.Enqueue(curr+Vector2.left);
                }
                if (!visited.Contains(curr+Vector2.down) && DistanceHelper.InRange(curr+Vector2.down, agentPos, maxDistance)) {
                    q.Enqueue(curr+Vector2.down);
                }
                if (!visited.Contains(curr+Vector2.right) && DistanceHelper.InRange(curr+Vector2.right, agentPos, maxDistance)) {
                    q.Enqueue(curr+Vector2.right);
                }
            }

            return res;
        }

        private bool IsBlocked(Vector2 pos) {
            return false;
        }
    }
}
