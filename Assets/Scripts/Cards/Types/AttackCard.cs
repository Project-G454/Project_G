using System.Collections.Generic;
using Cards.Data;
using Core.Loaders.Cards;
using Effects;
using Effects.Data;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class AttackCard: Card {
        public int damage { get; set; }
        public List<Effect> effects { get; set; }
        public int attackTimes { get; set; }

        public AttackCard(
            AttackCardData data
        ) : base(data) {
            this.damage = data.damage;
            this.effects = data.effects;
            this.attackTimes = data.attackTimes;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            for (int i = 0; i < this.attackTimes; i++) {
                Debug.Log("Player_" + sourceId.ToString() + " deal " + this.damage.ToString() + " damage to Player_" + targetId.ToString());
                CardPlayerInteraction.ApplyDamage(targetId, this.damage);
                foreach (Effect effect in effects) {
                    CardPlayerInteraction.ApplyEffect(targetId, effect);
                }
            }
        }

        public override void ApplyView(CardView view) {
            base.ApplyView(view);
            Sprite icon = CardDataLoader.LoadAttackIcon();
            view.CreateEffectDisplay(icon, damage);
            foreach (Effect effect in effects) {
                effect.Init();
                view.CreateEffectDisplay(effect.icon, effect.rounds);
            }
        }
    }
}

