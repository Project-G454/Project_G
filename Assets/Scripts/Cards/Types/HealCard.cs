using Cards.Data;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class HealCard: Card {
        public int healingAmount = 0;

        public HealCard(
            HealCardData data
        ) : base(data) {
            this.healingAmount = data.healingAmount;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log("Player_" + targetId.ToString() + " heal " + this.healingAmount.ToString() + " HP.");
            CardPlayerInteraction.ApplyHeal(targetId, this.healingAmount);
        }
    }
}

