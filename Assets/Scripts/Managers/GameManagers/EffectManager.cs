using Core.Entities;
using Core.Interfaces;
using Effects;
using Effects.Data;
using Effects.Factories;
using Entities;
using Events;
using Unity.VisualScripting;
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
            Trigger(entityId, new TurnStartEvent{entityId = entityId});
            EndTurn(entityId);
        }

        public void EndTurn(int entityId) {
            this.isTurnFinished = true;

            Trigger(entityId, new TurnEndEvent{entityId = entityId});
        }

        public void Trigger<T>(int entityId, T evt)
        {
            Entity entity = EntityManager.Instance.GetEntity(entityId);
            var allEffects = entity.GetEffectList();
            
            foreach (var effect in allEffects)
            {
                if (effect is IEventOn<T> typedEffect)
                {
                    typedEffect.On(evt);
                }
            }
        }
    }
}
