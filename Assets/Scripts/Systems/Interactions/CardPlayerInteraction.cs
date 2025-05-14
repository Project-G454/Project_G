using Core.Entities;
using Core.Managers;
using Entities;

namespace Systems.Interactions
{
    public class CardPlayerInteraction
    {
        public static void ApplyEffect(int targetId, int effectId) {
            EffectManager effectManager = EffectManager.Instance;
            effectManager.Apply(targetId, effectId);
        }

        public static void ApplyDamage(int targetId, int damage) {
            EntityManager entityManager = EntityManager.Instance;
            Entity target = entityManager.GetEntity(targetId);
            target.TakeDamage(damage);
        }

        public static void ApplyHeal(int targetId, int healingAmount) {
            Entity target = EntityManager.Instance.GetEntity(targetId);
            target.Heal(healingAmount);
        }

        public static void ApplyMove(int targetId, int step) {
            Entity target = EntityManager.Instance.GetEntity(targetId);
            target.energyManager.Add(step);
        }
        
    }
}
