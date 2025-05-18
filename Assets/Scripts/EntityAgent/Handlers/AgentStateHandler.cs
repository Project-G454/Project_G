using System;
using System.Linq;
using Core.Data;
using Core.Managers.Cards;
using Entities.Handlers;
using UnityEngine;

namespace Core.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        private AgentState _currentState = AgentState.Waiting;
        private AgentAction _actionState = AgentAction.End;
        private EntityAgent _agent;
        private bool _active = false;
        private bool _acting = false;
        private static readonly AgentAction[] _actionsWithCardAnimation = {
            AgentAction.Attack,
            AgentAction.Heal
        };
        private static readonly AgentAction[] _actionsWithMove = {
            AgentAction.Move,
            AgentAction.Escape
        };

        void Start() {
            _agent = GetComponent<EntityAgent>();
        }

        void Update() {
            Debug.Log($"Agent State: {_actionState}");
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
            if (!_acting) {
                Debug.Log($"Agent Action: {_actionState}");
                _agent.ExecuteStrategy(_actionState);
                _acting = true;
            }

            if (!_IsMovingEnd()) Debug.Log("Moving...");
            if (!_IsCardAnimationEnd()) Debug.Log("Using Card...");

            if (_IsMovingEnd() && _IsCardAnimationEnd()) {
                Debug.Log("End Action");
                ChangeState(AgentState.Waiting);
                _acting = false;
            }
        }

        private bool _IsMovingEnd() {
            if (!Enumerable.Contains(_actionsWithMove, _actionState)) return true;
            MoveHandler moveHandler = GetComponent<MoveHandler>();
            return (
                moveHandler != null &&
                !moveHandler.isMoving &&
                moveHandler.endMoving
            );
        }

        private bool _IsCardAnimationEnd() {
            if (!Enumerable.Contains(_actionsWithCardAnimation, _actionState)) return true;
            return (
                _agent != null &&
                _agent.strategy != null &&
                _agent.strategy.isCardAnimationEnd
            );
        }
    }
}
