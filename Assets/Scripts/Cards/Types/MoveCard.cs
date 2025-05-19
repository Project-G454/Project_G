using Cards.Data;
using Core.Loaders.Cards;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class EnergyCard: Card {
        public int step = 0;

        public EnergyCard(
            EnergyCardData data
        ) : base(data) {
            this.step = data.step;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log("Player_" + targetId.ToString() + " moved " + this.step.ToString() + " steps.");
            CardPlayerInteraction.ApplyMove(targetId, this.step);
        }

        public override void ApplyView(CardView view) {
            base.ApplyView(view);
            Sprite icon = CardDataLoader.LoadEnergyIcon();
            view.CreateEffectDisplay(icon, step);
        }
    }
}

