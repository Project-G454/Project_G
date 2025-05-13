using System.Collections.Generic;
using Agents.Helpers;
using Cards;
using Core.Entities;
using Core.Managers.Cards;
using Entities;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyAttack: AgentStrategy {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);

            // pick a card
            List<CardBehaviour> cardBehaviours = _GetUsableCards();
            if (cardBehaviours.Count == 0) return;
            int cardIdx = Random.Range(0, cardBehaviours.Count);
            CardBehaviour cardBehaviour = cardBehaviours[cardIdx];

            // choose a target
            List<int> targetIds = _GetReachableEntityIds(cardBehaviour.card.range);
            Debug.Log(targetIds.ToString());
            if (targetIds.Count == 0) return;
            int targetIdx = Random.Range(0, targetIds.Count);
            int targetId = targetIds[targetIdx];

            // use card
            base._UseCard(cardBehaviour, targetId);
        }

        // --- helper functions ---
        private List<CardBehaviour> _GetUsableCards() {
            // 找出可以使用的攻擊牌 (Attack/Magic)
            List<CardBehaviour> cardBehaviours = new();
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                Card card = cardBehaviour.card;
                if (cardBehaviour == null) continue;
                if (AgentCardHelper.IsAttackCard(card) && _agent.CanUseCard(card) && _agent.HasReachablePlayer(card.range)) {
                    cardBehaviours.Add(cardBehaviour);
                }
            }
            return cardBehaviours;
        }

        private List<int> _GetReachableEntityIds(int range) {
            // 找出可以攻擊的實體
            List<int> Ids = new();
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            foreach (Entity target in players) {
                if (
                    DistanceHelper.InRange(_agent.entity.position, target.position, range) &&
                    !target.IsDead()
                ) {
                    Ids.Add(target.entityId);
                }
            }
            return Ids;
        }
    }
}
