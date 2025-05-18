using System;
using System.Linq;
using Core.Data;
using Core.Managers.Cards;
using Entities.Handlers;
using UnityEngine;

namespace Core.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        private AgentState _currentState = AgentState.Waiting;
        private AgentAction _agentAction = AgentAction.End;
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
            CardManager.Instance.Unlock();
        }

        public void Unlock() {
            this._active = true;
            CardManager.Instance.Lock();
        }

        private void _ChangeState(AgentState newState) {
            _currentState = newState;
        }

        private void _HandleWaiting() {
            // 等到輪到自己行動
            if (_agent.IsTurnToAgent()) _ChangeState(AgentState.Planning);
        }

        private void _HandlePlanning() {
            _agentAction = _agent.DecisionStrategy();

            if (!_agent.CanUseStrategy(_agentAction)) {
                _agentAction = AgentAction.End;
            }
            _ChangeState(AgentState.Acting);
        }

        private void _HandleActing() {
            if (!_acting) {
                _agent.ExecuteStrategy(_agentAction);
                _acting = true;
            }

            if (_IsMovingEnd() && _IsCardAnimationEnd()) {
                _ChangeState(AgentState.Waiting);
                _acting = false;
            }
        }

        private bool _IsMovingEnd() {
            if (!Enumerable.Contains(_actionsWithMove, _agentAction)) return true;
            MoveHandler moveHandler = GetComponent<MoveHandler>();
            return (
                moveHandler != null &&
                !moveHandler.isMoving &&
                moveHandler.endMoving
            );
        }

        private bool _IsCardAnimationEnd() {
            if (!Enumerable.Contains(_actionsWithCardAnimation, _agentAction)) return true;
            return (
                _agent != null &&
                _agent.strategy != null &&
                _agent.strategy.isCardAnimationEnd
            );
        }
    }
}
