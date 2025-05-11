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
            Debug.Log($"Agent target to Player_{target.entityId}, Pos: {target.position}");
            MapManager.Instance.MoveTo(target.position);
        }

        // --- helper functions ---
    }
}
