using System;
using Agents.Data;
using Agents.Handlers;
using Agents.Helpers;
using Agents.Strategy;
using Cards;
using Cards.Data;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using UnityEngine;

namespace Agents {
    public class EntityAgent: MonoBehaviour {
        public Entity entity;
        public AgentDecision strategy;
        private AgentStateHandler _agentStateHandler;
        private AgentAction _actionState;

        public void Init() {
            this._agentStateHandler = GetComponent<AgentStateHandler>();
        }

        public void Bind(Entity entity) {
            this.entity = entity;
        }

        public AgentAction DecisionStrategy() {
            int attackCardRange = GetAttackCardRange();
            const int ESCAPE_RANGE = 5;

            if (!HasResource()) return AgentAction.End;
            else if (LowHP(0.5f)) {
                if (HasReachableEntity(ESCAPE_RANGE)) return AgentAction.Escape;
                else if (CanUseAnyCard()) return AgentAction.Heal;
                else return AgentAction.End;
            }
            else {
                if (!HasReachableEntity(attackCardRange)) return AgentAction.Move;
                else if (CanUseAnyCard()) return AgentAction.Attack;
                else return AgentAction.Move;
            }
        }

        public void ExecuteStrategy(AgentAction action) {
            AgentDecision strategy = null;
            switch (action) {
                case AgentAction.End:
                    break;
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
                default:
                    break;
            }
            strategy?.Execute(this);
        }

        // --- Helper functions ---
        public bool IsAgentTurn() {
            return BattleManager.Instance.currentEntity.entityId == entity.entityId;
        }

        public bool HasResource() {
            return entity.energyManager.energy > 0;
        }

        public bool CanUseAnyCard() {
            int energy = entity.energyManager.energy;
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                if (cardBehaviour.card.cost < energy) return true;
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
                    if (!HasResource()) return false;
                    break;
            }
            return true;
        }

        public bool LowHP(float threshold=0.5f) {
            return entity.currentHp <= entity.maxHp * threshold;
        }

        public bool HasReachableEntity(int range) {
            foreach (Entity target in EntityManager.Instance.GetEntityList()) {
                if (entity.entityId == target.entityId) continue;
                if (DistanceHelper.InRange(entity.position, target.position, range)) {
                    return true;
                }
            }
            return false;
        }

        public int GetAttackCardRange() {
            int maxRange = 0;
            foreach (GameObject cardObj in CardManager.cardList) {
                Card card = cardObj.GetComponent<Card>();
                if (AgentCardHelper.IsAttackCard(card)) {
                    maxRange = Math.Max(maxRange, card.range);
                }
            }
            return maxRange;
        }

        public bool IsAttackCardUsable() {
            foreach (GameObject cardObj in CardManager.cardList) {
                Card card = cardObj.GetComponent<Card>();
                if (!AgentCardHelper.IsAttackCard(card) || !CanUseCard(card)) continue;
                if (HasReachableEntity(card.range)) return true;
            }
            return false;
        }

        public bool IsHealCardUsable() {
            foreach (GameObject cardObj in CardManager.cardList) {
                Card card = cardObj.GetComponent<Card>();
                if (!AgentCardHelper.IsHealCard(card) || !CanUseCard(card)) continue;
                else return true;
            }
            return false;
        }

        public bool CanUseCard(Card card) {
            return card.cost < entity.energyManager.energy;
        }
    }
}
