using Core.Managers.Cards;

namespace Agents.Strategies {
    class StrategyEnd: AgentStrategy {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            if (!CardManager.Instance.isTurnFinished) {
                agent.ResetState();
                CardManager.Instance.EndTurn();
                agent.stateHandler.Lock();
            }
        }

        // --- helper functions ---
    }
}
