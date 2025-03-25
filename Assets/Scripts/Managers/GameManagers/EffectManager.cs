using Core.Entities;
using Core.Interfaces;
using Effects;
using Effects.Data;
using Effects.Factories;
using Entities;
using UnityEngine;

namespace Core.Managers {
    public class EffectManager: MonoBehaviour, IManager {
        public static EffectManager Instance;
        public bool isTurnFinished = true;
        private EntityManager _entityManager;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            _entityManager = EntityManager.Instance;
        }

        public void Apply(int targetId, int effectId) {
            Entity entity = _entityManager.GetEntity(targetId);
            EffectData effectData = EffectFactory.GetFakeEffect();
            Effect effect = EffectFactory.MakeEffect(effectData);
            Debug.Log($"Apply Effect_{effect.effectId} -> Entity_{entity.entityId}");
            // entity.AddEffect(effect);
        }

        public void StartTurn(int entityId) {
            this.isTurnFinished = false;

            // Iterating the effects recorded in the entity
            // effect.Trigger(entityId);
            EndTurn();
        }

        public void EndTurn() {
            this.isTurnFinished = true;
        }
    }
}
