namespace Agents.Data {
    public enum AgentState {
        Waiting,
        Planning,
        Acting
    }

    public enum AgentAction {
        Idle,
        Move,
        Attack,
        Heal
    }
}
