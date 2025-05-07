namespace Agents {
    public abstract class AgentDecision {
        protected EntityAgent _agent;
        public virtual void Execute(EntityAgent agent) {
            _agent = agent;
        }
    }
}
