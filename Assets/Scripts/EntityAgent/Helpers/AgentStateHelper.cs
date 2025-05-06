using System.Collections.Generic;
using Cards;
using Core.Managers;
using Core.Managers.Cards;
using Effects;
using Entities;
using UnityEngine;

namespace Agents.Helpers {
    public class AgentStateHelper {
        public static bool IsAgentTurn(EntityAgent agent) {
            return BattleManager.Instance.currentEntity.entityId == agent.entity.entityId;
        }

        public static bool HasResource(EntityAgent agent) {
            return agent.entity.energyManager.energy > 0;
        }

        public static bool CanUseCard(EntityAgent agent) {
            int energy = agent.entity.energyManager.energy;
            foreach (GameObject cardObj in CardManager.cardList) {
                CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
                if (cardBehaviour.card.cost < energy) return true;
            }
            return false;
        }
    }
}
