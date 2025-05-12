using System;
using Agents.Data;
using Core.Managers.Cards;
using Entities.Handlers;
using UnityEngine;

namespace Agents.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        private AgentState _currentState = AgentState.Waiting;
        private AgentAction _actionState = AgentAction.End;
        private EntityAgent _agent;
        private bool _active = false;

        void Start() {
            _agent = GetComponent<EntityAgent>();
        }

        void Update() {
            if (!this._active) return;
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

        public void Lock() {
            this._active = false;
        }

        public void Unlock() {
            this._active = true;
        }

        private void ChangeState(AgentState newState) {
            // Debug.Log($"Agent state: {_currentState} → {newState}");
            _currentState = newState;
        }

        private void _HandleWaiting() {
            // 等到輪到自己行動
            if (_agent.IsTurnToAgent()) ChangeState(AgentState.Planning);
        }

        private void _HandlePlanning() {
            _actionState = _agent.DecisionStrategy();
            Debug.Log($"Agent Select Action: {_actionState}");
            if (!_agent.CanUseStrategy(_actionState)) {
                Debug.Log($"Action {_actionState} error");
                _actionState = AgentAction.End;
            }
            ChangeState(AgentState.Acting);
        }

        private void _HandleActing() {
            Debug.Log($"Agent Action: {_actionState}");
            _agent.ExecuteStrategy(_actionState);

            if (!IsMoving()) {
                if (_actionState == AgentAction.End) _EndAction();
                ChangeState(AgentState.Waiting);
            }
        }

        private bool IsMoving() {
            MoveHandler moveHandler = GetComponent<MoveHandler>();
            return moveHandler != null && moveHandler.isMoving;
        }

        private void _EndAction() {
            if (!CardManager.Instance.isTurnFinished) {
                _agent.ResetState();
                CardManager.Instance.EndTurn();
                Lock();
            }
        }
    }
}
