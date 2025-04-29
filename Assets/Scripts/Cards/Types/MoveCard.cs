using Cards.Data;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class MoveCard: Card {
        public int step = 0;

        public MoveCard(
            MoveCardData data
        ) : base(data) {
            this.step = data.step;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log("Player_" + targetId.ToString() + " moved " + this.step.ToString() + " steps.");
            CardPlayerInteraction.ApplyMove(targetId, this.step);
        }
    }
}

