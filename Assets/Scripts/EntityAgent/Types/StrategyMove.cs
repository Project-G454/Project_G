using System.Collections.Generic;
using Core.Entities;
using Core.Managers;
using Entities;
using Entities.Handlers;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyMove: AgentStrategy {
        public override bool Execute(EntityAgent agent) {
            base.Execute(agent);
            Entity target = _FindNearestPlayer();
            if (target == null) return false;
            Vector2 bestPos = base._FindBestPosition(target.position, 5);
            if (!CanMoveTo(bestPos)) {
                _agent.canMove = false;
                return false;
            }
            Debug.Log($"Agent trying move to Player_{target.entityId}, Pos: {bestPos}");
            MapManager.Instance.MoveTo(bestPos);
            return false;
        }

        // --- helper functions ---
    }
}
