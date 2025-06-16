using Cards.Data;
using Core.Loaders.Cards;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class SummonCard: Card {
        public int summonAmount = 0;

        public SummonCard(
            SummonCardData data
        ) : base(data) {
            this.summonAmount = data.summonAmount;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log("Player_" + targetId.ToString() + " summon " + this.summonAmount.ToString() + " Entities.");
            CardPlayerInteraction.ApplySummon(targetId, this.summonAmount);
        }

        public override void ApplyView(CardView view) {
            base.ApplyView(view);
            Sprite icon = CardDataLoader.LoadHealIcon();
            view.CreateEffectDisplay(icon, summonAmount);
        }
    }
}

