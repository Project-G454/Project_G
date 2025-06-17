using System;
using System.Collections.Generic;
using Agents.Data;
using Agents.Handlers;
using Agents.Helpers;
using Core.Handlers;
using Agents.Strategies;
using Cards;
using Cards.Data;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using NUnit.Framework;
using UnityEngine;

namespace Agents {
    public class EntityAgent: MonoBehaviour {
        public Entity entity;
        public AgentStrategy strategy;
        public AgentStateHandler stateHandler;
        private bool _isBinded = false;
        public bool canMove = true;
        public bool endMoving = true;
        private bool forceEndAction = false;

        public void Start() {
            Bind();
        }

        public void Bind() {
            if (_isBinded) return;
            EntityBehaviour entityBehaviour = GetComponent<EntityBehaviour>();
            stateHandler = GetComponent<AgentStateHandler>();
            if (stateHandler == null) {
                stateHandler = gameObject.AddComponent<AgentStateHandler>();
            }

            entity = entityBehaviour.entity;
            entity.type = EntityTypes.ENEMY;

            Debug.Log($"Bind Agent to Entity_{entity.entityId}");
            _isBinded = true;
        }

        public void ResetState() {
            canMove = true;
            forceEndAction = false;
        }

        public AgentAction DecisionStrategy() {
            const int ESCAPE_RANGE = 5;

            if (!HasResource() || forceEndAction) {
                Debug.Log("No Resource");
                return AgentAction.End;
            }
            else if (LowHP(0.5f) && !IsHealCardUsable() && IsAttackCardUsable()) {
                return AgentAction.Attack;
            }
            else if (LowHP(0.5f)) {
                if (HasReachablePlayer(ESCAPE_RANGE)) return AgentAction.Escape;
                else if (IsHealCardUsable()) return AgentAction.Heal;
                else {
                    Debug.Log("Can not use Heal Card");
                    return AgentAction.End;
                }
            }
            else {
                if (IsAttackCardUsable()) return AgentAction.Attack;
                else if (IsSummonCardUsable()) return AgentAction.Summon;
                else if (HasResource() && !HasReachablePlayer(1)) return AgentAction.Move;
                else {
                    Debug.Log("Can not use Attack Card");
                    return AgentAction.End;
                }
            }
        }

        public bool ExecuteStrategy(AgentAction action) {
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
                case AgentAction.Summon:
                    strategy = new StrategySummon();
                    break;
                case AgentAction.End:
                default:
                    strategy = new StrategyEnd();
                    break;
            }

            bool success = strategy?.Execute(this) ?? false;
            if (!success) forceEndAction = true;

            return true;
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
                    if (!HasResource() || !canMove) return false;
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
                if (DistanceHelper.EntityInRange(gameObject, target, range)) return true;
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

        public bool IsSummonCardUsable() {
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                Card card = cardBehaviour.card;
                if (AgentCardHelper.IsSummonCard(card) && CanUseCard(card)) return true;
            }
            return false;
        }

        public bool CanUseCard(Card card) {
            return card.cost <= entity.energyManager.energy;
        }
    }
}
