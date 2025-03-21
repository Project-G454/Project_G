using Cards.Interactions;

namespace Cards.Categories {
    public class MoveCard: Card {
        public int step = 0;

        public MoveCard(
            CardData data,
            int step
        ) : base(data) {
            this.type = CardTypes.MOVE;
            this.step = step;
        }

        public override void Use(int sourceId, int targetId) {
            base.Use(sourceId, targetId);
            CardPlayerInteraction.ApplyMove(targetId, this.step);
        }
    }
}

