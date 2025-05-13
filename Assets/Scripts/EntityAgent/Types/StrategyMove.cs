using System.Collections.Generic;
using Core.Entities;
using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategy {
    class StrategyMove: AgentDecision {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            Entity target = _FindNearestTarget();
            if (target == null) return;
            MapManager.Instance.MoveTo(target.position);
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
    }
}
