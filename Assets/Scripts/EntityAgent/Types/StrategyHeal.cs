using System.Collections.Generic;
using Agents.Helpers;
using Cards;
using Core.Managers.Cards;
using UnityEngine;

namespace Agents.Strategies {
    class StrategyHeal: AgentStrategy {
        public override void Execute(EntityAgent agent) {
            base.Execute(agent);
            List<CardBehaviour> cardBehaviours = _GetUsableCards();
            if (cardBehaviours.Count == 0) return;
            int cardIdx = Random.Range(0, cardBehaviours.Count);
            CardBehaviour cardBehaviour = cardBehaviours[cardIdx];
            Debug.Log($"Agent selected {cardBehaviour.card.cardName}");

            // use card
            CardManager.Instance.UseCard(cardBehaviour, agent.entity.entityId);
        }

        private List<CardBehaviour> _GetUsableCards() {
            // 找出可以使用的治療牌 (Heal)
            List<CardBehaviour> cardBehaviours = new();
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                Card card = cardBehaviour.card;
                if (cardBehaviour == null) continue;
                if (AgentCardHelper.IsHealCard(card) && _agent.CanUseCard(card)) {
                    cardBehaviours.Add(cardBehaviour);
                }
            }
            return cardBehaviours;
        }
    }
}
