using System;
using System.Linq;
using Agents.Data;
using Core.Managers.Cards;
using Entities.Handlers;
using UnityEngine;

namespace Agents.Handlers {
    public class AgentStateHandler : MonoBehaviour {
        [SerializeField] private AgentState _currentState = AgentState.Waiting;
        [SerializeField] private AgentAction _agentAction = AgentAction.End;
        [SerializeField] private EntityAgent _agent;
        [SerializeField] private bool _active = false;
        [SerializeField] private bool _acting = false;
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

            Debug.Log($"Executing strategy: {_agentAction}");
            if (!_agent.CanUseStrategy(_agentAction)) {
                Debug.Log("Agent can not use strategy...");
                _agentAction = AgentAction.End;
            }
            _ChangeState(AgentState.Acting);
        }

        private void _HandleActing() {
            if (!_acting) {
                _acting = _agent.ExecuteStrategy(_agentAction);
                if (!_acting) _ChangeState(AgentState.Waiting);
                Debug.Log("Done.");
            }

            if (_IsMovingEnd() && _IsCardAnimationEnd()) {
                _ChangeState(AgentState.Waiting);
                _acting = false;
            }
        }

        private bool _IsMovingEnd() {
            MoveHandler moveHandler = GetComponent<MoveHandler>();
            if (!Enumerable.Contains(_actionsWithMove, _agentAction)) return true;
            return moveHandler == null || !moveHandler.isMoving;
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

