using Cards;
using Cards.Data;

namespace Agents.Helpers {
    class AgentCardHelper {
        public static bool IsAttackCard(Card card) {
            return card.type == CardTypes.ATTACK || card.type == CardTypes.MAGIC;
        }

        public static bool IsHealCard(Card card) {
            return card.type == CardTypes.HEAL;
        }

        public static bool IsSummonCard(Card card) {
            return card.type == CardTypes.SUMMON || card.type == CardTypes.MAGIC;
        }
    }
}
