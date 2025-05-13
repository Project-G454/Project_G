using System.Collections.Generic;
using Core.Entities;
using Core.Managers;
using Entities;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyMove: AgentStrategy {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            Entity target = _FindNearestPlayer();
            if (target == null) return;
            Vector2 bestPos = base._FindBestPosition(target.position, 5);
            Debug.Log($"Agent trying move to Player_{target.entityId}, Pos: {bestPos}");
            MapManager.Instance.MoveTo(bestPos);
        }

        // --- helper functions ---
    }
}
