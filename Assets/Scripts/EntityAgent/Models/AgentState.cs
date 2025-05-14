namespace Agents.Data {
    public enum AgentState {
        Waiting,
        Planning,
        Acting
    }

    public enum AgentAction {
        End,
        Move,
        Escape,
        Attack,
        Heal
    }
}
