using Cards.Interactions;
using UnityEngine;

namespace Cards.Categories {
    public class MagicCard: Card {
        public int effectId;

        public MagicCard(
            CardData data,
            int effectId
        ) : base(data) {
            this.type = CardTypes.MAGIC;
            this.effectId = effectId;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            CardPlayerInteraction.ApplyEffect(targetId, this.effectId);
        }
    }
}

