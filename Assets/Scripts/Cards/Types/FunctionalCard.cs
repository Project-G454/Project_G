using Cards.Data;
using Core.Loaders.Cards;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class FunctionalCard: Card {
        public int drawCount = 0;

        public FunctionalCard(
            FunctionalCardData data
        ) : base(data) {
            drawCount = data.drawCount;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log($"Entity_{sourceId} draw {this.drawCount} Cards");
            CardPlayerInteraction.ApplyDraw(targetId, this.drawCount);
        }

        public override void ApplyView(CardView view) {
            base.ApplyView(view);
        }
    }
}

