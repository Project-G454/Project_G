using System.Collections.Generic;
using Agents.Helpers;
using Cards;
using Core.Entities;
using Core.Managers.Cards;
using Entities;
using UnityEngine;

namespace Agents.Strategy {
    class StrategyAttack: AgentDecision {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);

            // pick a card
            List<CardBehaviour> cardBehaviours = _GetUsableCards();
            if (cardBehaviours.Count == 0) return;
            int cardIdx = Random.Range(0, cardBehaviours.Count - 1);
            CardBehaviour cardBehaviour = cardBehaviours[cardIdx];

            // choose a target
            List<int> targetIds = _GetReachableEntityIds(cardBehaviour.card.range);
            if (targetIds.Count == 0) return;
            int targetIdx = Random.Range(0, targetIds.Count - 1);
            int targetId = targetIds[targetIdx];

            // use card
            CardManager.Instance.UseCard(cardBehaviour, targetId);
        }

        // --- helper functions ---
        private List<CardBehaviour> _GetUsableCards() {
            // 找出可以使用的攻擊牌 (Attack/Magic)
            List<CardBehaviour> cardBehaviours = new();
            foreach (GameObject cardObj in CardManager.cardList) {
                Card card = cardObj.GetComponent<Card>();
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                if (cardBehaviour == null) continue;
                if (AgentCardHelper.IsAttackCard(card) && _agent.CanUseCard(card) && _agent.HasReachableEntity(card.range)) {
                    cardBehaviours.Add(cardBehaviour);
                }
            }
            return cardBehaviours;
        }

        private List<int> _GetReachableEntityIds(int range) {
            // 找出可以攻擊的實體
            List<int> Ids = new();
            foreach (Entity target in EntityManager.Instance.GetEntityList()) {
                if (_agent.entity.entityId == target.entityId) continue;
                if (DistanceHelper.InRange(_agent.entity.position, target.position, range)) {
                    Ids.Add(target.entityId);
                }
            }
            return Ids;
        }
    }
}
