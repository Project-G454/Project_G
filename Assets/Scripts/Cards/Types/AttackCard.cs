using Cards.Data;
using Systems.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class AttackCard: Card {
        public int damage { get; set; }
        public float criticalChance { get; set; }
        public float criticalMultiplier { get; set; }
        public int splashRadius { get; set; }
        public int piercing { get; set; }
        public int effectId { get; set; }
        public int attackTimes { get; set; }

        public AttackCard(
            AttackCardData data
        ): base(data) {
            this.damage = data.damage;
            this.effectId = data.effectId;
            this.attackTimes = data.attackTimes;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            for (int i=0; i<this.attackTimes; i++) {
                Debug.Log("Player_" + sourceId.ToString() + " deal " + this.damage.ToString() + " damage to Player_" + targetId.ToString());
                CardPlayerInteraction.ApplyDamage(targetId, this.damage);
                if (this.effectId > 0) CardPlayerInteraction.ApplyEffect(targetId, (int)this.effectId);
            }
        }
    }
}

