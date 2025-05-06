using Agents.Data;
using Cards;
using Entities;
using UnityEngine;

namespace Agents {
    public class EntityAgent: MonoBehaviour {
        public Entity entity;
        public CardBehaviour cardBehaviour;
        public AgentDecision strategy;
        private AgentState _agentState;
        private AgentAction _actionState;

        public void Bind(Entity entity, CardBehaviour cardBehaviour) {
            this.entity = entity;
            this.cardBehaviour = cardBehaviour;
        }

        public void DecisionStrategy() {}
    }
}
