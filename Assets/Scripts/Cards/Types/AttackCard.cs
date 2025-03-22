using Cards.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class AttackCard: Card {
        public int damage { get; set; }
        public float criticalChance { get; set; }
        public float criticalMultiplier { get; set; }
        public int splashRadius { get; set; }
        public int piercing { get; set; }

        public AttackCard(
            AttackCardData data
        ): base(data) {
            this.type = CardTypes.ATTACK;
            this.damage = data.damage;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            Debug.Log("Player_" + sourceId.ToString() + " deal " + this.damage.ToString() + " damage to Player_" + targetId.ToString());
            CardPlayerInteraction.ApplyDamage(targetId, this.damage);
        }
    }
}

