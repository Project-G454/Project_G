using System;
using Agents.Data;
using Agents.Helpers;
using UnityEngine;

namespace Agents.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        private AgentState _currentState = AgentState.Waiting;
        private EntityAgent _agent;

        void Update() {
            switch (_currentState) {
                case AgentState.Waiting:
                    _HandleWaiting();
                    break;
                case AgentState.Planning:
                    _HandlePlanning();
                    break;
                case AgentState.Acting:
                    _HandleActing();
                    break;
            }
        }

        private void ChangeState(AgentState newState) {
            Debug.Log($"Agent state: {_currentState} → {newState}");
            _currentState = newState;
        }

        private void _HandleWaiting() {
            // 等到輪到自己行動
            if (AgentStateHelper.IsAgentTurn(_agent)) ChangeState(AgentState.Planning);
        }

        private void _HandlePlanning() {
            _agent.DecisionStrategy();
            ChangeState(AgentState.Acting);
        }

        private void _HandleActing() {
            _agent.strategy.Execute();
            _EndTurn();
            ChangeState(AgentState.Waiting);
        }

        private void _EndTurn() {}
    }
}
