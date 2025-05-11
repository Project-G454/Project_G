using System.Collections.Generic;
using Core.Entities;
using Entities;
using UnityEngine;

namespace Agents {
    public abstract class AgentStrategy {
        protected EntityAgent _agent;

        public virtual void Execute(EntityAgent agent) {
            _agent = agent;
        }

        protected Entity _FindNearestPlayer() {
            // 找出距離最近的玩家
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            Entity target = null;
            float minDistance = 10;
            foreach (Entity entity in players) {
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
