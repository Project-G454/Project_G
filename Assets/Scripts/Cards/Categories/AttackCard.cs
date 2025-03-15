using Cards.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class AttackCard: Card
    {
        public int damage { get; set; }
        public float criticalChance { get; set; }
        public float criticalMultiplier { get; set; }
        public int splashRadius { get; set; }
        public int piercing { get; set; }

        public AttackCard(
            CardData data,
            int damage = 0
        ): base(data) {
            this.type = CardTypes.ATTACK;
            this.damage = damage;
        }

        public override void Use(int sourceId, int targetId)
        {
            base.Use(sourceId, targetId);
            CardPlayerInteraction.ApplyDamage(targetId, this.damage);
        }
    }
}

