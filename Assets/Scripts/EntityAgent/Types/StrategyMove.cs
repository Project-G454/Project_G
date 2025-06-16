using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyMove: AgentStrategy {
        public override bool Execute(EntityAgent agent) {
            base.Execute(agent);
            agent.endMoving = false;
            Debug.Log("Set moving");
            Entity target = _FindNearestPlayer();
            if (target == null) return false;
            Vector2 bestPos = base._FindBestPosition(target.position, 5);
            if (!CanMoveTo(bestPos)) {
                _agent.canMove = false;
                agent.endMoving = true;
                return false;
            }
            Debug.Log($"Agent trying move to Player_{target.entityId}, Pos: {bestPos}");
            MapManager.Instance.MoveTo(bestPos, () => {
                agent.endMoving = true;
                Debug.Log("Agent moving end");
            });
            return false;
        }

        // --- helper functions ---
    }
}
