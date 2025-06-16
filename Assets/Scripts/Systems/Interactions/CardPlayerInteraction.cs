using System.Collections.Generic;
using Core.Entities;
using Core.Managers;
using Effects;
using Entities;
using UnityEngine;

namespace Systems.Interactions
{
    public class CardPlayerInteraction {
        public static void ApplyEffect(int targetId, Effect effect) {
            EffectManager effectManager = EffectManager.Instance;
            effectManager.Apply(targetId, effect);
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

        public static void ApplySummon(int targetId, int summonAmount) {
            List<EntityData> entityDatas = new();
            for (int i = 0; i < summonAmount; i++) {
                entityDatas.Add(new EntityData(
                    20,
                    20,
                    "Minion",
                    EntityTypes.ENEMY,
                    EntityClasses.Minion
                ));
            }

            List<Vector3> spawnPositions = GridManager.Instance.GetSpawnPositions(entityDatas.Count);

            for (int i = 0; i < entityDatas.Count; i++) {
                var entity = EntityManager.Instance.CreateEntity(entityDatas[i], spawnPositions[i]);
                BattleManager.Instance.AddTurnOreder(entity.entityId);
            }

            BattleManager.Instance.BindAgents();
        }
    }
}
