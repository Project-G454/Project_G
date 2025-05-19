using System.Collections.Generic;
using Cards.Data;
using Core.Managers;
using Effects;
using Effects.Data;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class MagicCard: Card {
        public List<Effect> effects { get; set; }

        public MagicCard(
            MagicCardData data
        ) : base(data) {
            this.effects = data.effects;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);

            foreach (Effect effect in effects) {
                Debug.Log("Player_" + sourceId.ToString() + " used Magic_" + effect.id.ToString() + ".");
                CardPlayerInteraction.ApplyEffect(targetId, effect);
            }
        }
        
        public override void ApplyView(CardView view) {
            base.ApplyView(view);
            foreach (Effect effect in effects) {
                effect.Init();
                view.CreateEffectDisplay(effect.icon, effect.rounds);
            }
        }
    }
}

