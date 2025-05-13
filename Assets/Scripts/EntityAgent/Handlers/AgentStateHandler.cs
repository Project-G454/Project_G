using System;
using Agents.Data;
using UnityEngine;

namespace Agents.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        private AgentState _currentState = AgentState.Waiting;
        private AgentAction _actionState = AgentAction.End;
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
            if (_agent.IsAgentTurn()) ChangeState(AgentState.Planning);
        }

        private void _HandlePlanning() {
            _actionState = _agent.DecisionStrategy();
            if (!_agent.CanUseStrategy(_actionState)) {
                _actionState = AgentAction.End;
            }
            ChangeState(AgentState.Acting);
        }

        private void _HandleActing() {
            if (_actionState != AgentAction.End) {
                _agent.ExecuteStrategy(_actionState);
            }
            _EndAction();
            ChangeState(AgentState.Waiting);
        }

        private void _EndAction() {}
    }
}
