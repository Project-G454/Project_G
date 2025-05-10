using System.Collections.Generic;
using Core.Entities;
using Entities;
using UnityEngine;

namespace Agents {
    public abstract class AgentDecision {
        protected EntityAgent _agent;
        public virtual void Execute(EntityAgent agent) {
            _agent = agent;
        }

        protected Entity _FindNearestTarget() {
            // 找出距離最近的玩家
            List<Entity> entities = EntityManager.Instance.GetEntityList();
            float minDistance = 10;
            Entity target = null;
            foreach (Entity entity in entities) {
                if (entity.entityData.type == EntityTypes.ENEMY) continue;
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
