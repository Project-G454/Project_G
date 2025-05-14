using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Core.Loaders.Effects;
using Effects;
using Effects.Data;
using Effects.Factories;
using Entities;
using Entities.Animations;
using Events;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Managers {
    public class EffectManager: MonoBehaviour, IManager {
        public static EffectManager Instance;
        public bool isTurnFinished = true;
        private EntityManager _entityManager;
        private BattleManager _battleManager;
        public Dictionary<int, EffectData> effectDict = new();

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
            _battleManager = BattleManager.Instance;

            List<EffectData> effects = EffectDataLoader.LoadAll();
            effects.ForEach(e => {
                effectDict.Add(e.id, e);
            });
        }

        public EffectData GetEffectbyId(int id) {
            return effectDict.GetValueOrDefault(id);
        }

        public void Apply(int targetId, int effectId) {
            Entity entity = _entityManager.GetEntity(targetId);
            EffectData effectData = GetEffectbyId(effectId);
            Effect effect = EffectFactory.MakeEffect(effectData, targetId);
            Debug.Log($"Apply Effect_{effect.id} -> Entity_{entity.entityId}");
            entity.AddEffect(effect);
        }

        public void BeforeTurn() {
            int entityId = _battleManager.currentEntity.entityId;
            isTurnFinished = false;

            _Trigger(entityId, new BeforeTurnEvent());
            
            isTurnFinished = true;
        }

        public void AfterTurn() {
            int entityId = _battleManager.currentEntity.entityId;
            isTurnFinished = false;

            _Trigger(entityId, new AfterTurnEvent());
            _ReduceEffect(entityId);

            isTurnFinished = true;
        }

        private void _Trigger<T>(int entityId, T evt) {
            Entity entity = EntityManager.Instance.GetEntity(entityId);
            List<Effect> allEffects = entity.GetEffectList();

            allEffects.ForEach(effect => {
                if (effect is IEventOn<T> typedEffect) {
                    typedEffect.On(evt);
                    EntityAnimation.PlayAnimationOnce(EntityManager.Instance.GetEntityObject(entityId), PlayerState.DAMAGED);
                    if (entity.IsDead()) {
                        EntityAnimation.PlayAnimation(EntityManager.Instance.GetEntityObject(entityId), PlayerState.DEATH);
                    }
                }
            });
        }

        private void _ReduceEffect(int entityId) {
            Entity entity = EntityManager.Instance.GetEntity(entityId);
            List<Effect> allEffects = entity.GetEffectList();
            List<Effect> expiredEffects = new();

            allEffects.ForEach(effect => {
                effect.rounds--;
                if (effect.rounds <= 0) expiredEffects.Add(effect);
            });

            expiredEffects.ForEach(e => entity.RemoveEffect(e));
        }
    }
}
