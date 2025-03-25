using Core.Entities;
using Effects;
using Effects.Data;
using Effects.Factories;
using Entities;
using UnityEngine;

namespace Core.Managers {
    class EffectManager: MonoBehaviour {
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

        public void Start() {
            Init();
        }

        public void Apply(int targetId, int effectId) {
            Entity entity = _entityManager.GetEntity(targetId);
            EffectData effectData = EffectFactory.GetFakeEffect();
            Effect effect = EffectFactory.MakeEffect(effectData);
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
