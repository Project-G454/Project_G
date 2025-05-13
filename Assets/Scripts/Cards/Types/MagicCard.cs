using Cards.Data;
using Core.Managers;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class MagicCard: Card {
        public int effectId;

        public MagicCard(
            MagicCardData data
        ) : base(data) {
            this.effectId = data.effectId;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            
            Debug.Log("Player_" + sourceId.ToString() + " used Magic_" + this.effectId.ToString() + ".");
            CardPlayerInteraction.ApplyEffect(targetId, this.effectId);
        }
    }
}

