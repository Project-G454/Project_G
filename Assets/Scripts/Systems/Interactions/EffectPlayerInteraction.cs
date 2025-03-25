using Core.Entities;
using Entities;

namespace Systems.Interactions
{
    public class EffectPlayerInteraction
    {
        public static void ApplyEffect(int targetId, int effectId) {}

        public static void ApplyDamage(int targetId, int damage) {
            EntityManager entityManager = EntityManager.Instance;
            Entity target = entityManager.GetEntity(targetId);
            target.TakeDamage(damage);
        }

        public static void ApplyHeal(int targetId, int healing) {}

        public static void ApplyMove(int targetId, int step) {}
        
    }
}
