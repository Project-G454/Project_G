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
    }
}
