using System.Collections.Generic;
using Cards.Data;
using Core.Managers;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class MagicCard: Card {
        public List<int> effectIds;

        public MagicCard(
            MagicCardData data
        ) : base(data) {
            this.effectIds = data.effectIds;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            
            foreach (int effectId in this.effectIds) {
                Debug.Log("Player_" + sourceId.ToString() + " used Magic_" + effectId.ToString() + ".");
                CardPlayerInteraction.ApplyEffect(targetId, effectId);
            }
        }
    }
}

