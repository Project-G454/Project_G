using System;
using System.Collections.Generic;
using Core.Data;
using Core.Handlers;
using Core.Helpers;
using Core.Strategies;
using Cards;
using Cards.Data;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using NUnit.Framework;
using UnityEngine;

namespace Core {
    public class EntityAgent: MonoBehaviour {
        public Entity entity;
        public AgentStrategy strategy;
        public AgentStateHandler stateHandler;
        private bool _isBinded = false;
        public bool canMove = true;

        public void Start() {
            Bind();
        }

        public void Bind() {
            if (_isBinded) return;
            EntityBehaviour entityBehaviour = GetComponent<EntityBehaviour>();
            this.stateHandler = GetComponent<AgentStateHandler>();

            this.entity = entityBehaviour.entity;
            this.entity.type = EntityTypes.ENEMY;

            Debug.Log($"Bind Agent to Entity_{entity.entityId}");
            _isBinded = true;
        }

        public void ResetState() {
            canMove = true;
        }

        public AgentAction DecisionStrategy() {
            const int ESCAPE_RANGE = 5;

            if (!HasResource()) {
                Debug.Log("No Resource");
                return AgentAction.End;
            }
            else if (LowHP(0.5f)) {
                if (HasReachablePlayer(ESCAPE_RANGE) && canMove) return AgentAction.Escape;
                else if (IsHealCardUsable()) return AgentAction.Heal;
                else {
                    Debug.Log("Can not use Heal Card");
                    return AgentAction.End;
                }
            }
            else {
                if (IsAttackCardUsable()) return AgentAction.Attack;
                else if (HasResource() && !HasReachablePlayer(1)) return AgentAction.Move;
                else {
                    Debug.Log("Can not use Attack Card");
                    return AgentAction.End;
                }
            }
        }

        public void ExecuteStrategy(AgentAction action) {
            AgentStrategy strategy = null;
            switch (action) {
                case AgentAction.Move:
                    strategy = new StrategyMove();
                    break;
                case AgentAction.Attack:
                    strategy = new StrategyAttack();
                    break;
                case AgentAction.Escape:
                    strategy = new StrategyEscape();
                    break;
                case AgentAction.Heal:
                    strategy = new StrategyHeal();
                    break;
                case AgentAction.End:
                default:
                    strategy = new StrategyEnd();
                    break;
            }
            strategy?.Execute(this);
        }

        // --- Helper functions ---
        public bool IsTurnToAgent() {
            var manager = BattleManager.Instance;
            if (manager == null || manager.currentEntity == null) return false;
            return BattleManager.Instance.currentEntity.entityId == entity.entityId;
        }

        public bool HasResource() {
            return entity.energyManager.energy > 0;
        }

        public bool CanUseAnyCard() {
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                if (CanUseCard(cardBehaviour.card)) return true;
            }
            return false;
        }

        public bool CanUseStrategy(AgentAction strategy) {
            switch (strategy) {
                case AgentAction.Attack:
                    if (!IsAttackCardUsable()) return false;
                    break;
                case AgentAction.Heal:
                    if (!IsHealCardUsable()) return false;
                    break;
                case AgentAction.Move:
                    if (!HasResource()) return false;
                    break;
                case AgentAction.Escape:
                    if (!HasResource() || !canMove) return false;
                    break;
            }
            return true;
        }

        public bool LowHP(float threshold=0.5f) {
            return entity.currentHp <= entity.maxHp * threshold;
        }

        public bool HasReachablePlayer(int range) {
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            foreach (Entity target in players) {
                if (DistanceHelper.InRange(entity.position, target.position, range) && !target.IsDead()) {
                    return true;
                }
            }
            return false;
        }

        public bool IsAttackCardUsable() {
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                Card card = cardBehaviour.card;
                if (
                    AgentCardHelper.IsAttackCard(card) &&
                    CanUseCard(card) &&
                    HasReachablePlayer(card.range)
                ) return true;
            }
            return false;
        }

        public bool IsHealCardUsable() {
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                Card card = cardBehaviour.card;
                if (AgentCardHelper.IsHealCard(card) && CanUseCard(card)) return true;
            }
            return false;
        }

        public bool CanUseCard(Card card) {
            return card.cost <= entity.energyManager.energy;
        }
    }
}
