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

        private void Awake() {
            Instance = this;
        }

        public void Reset() {}

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

        public void Apply(int targetId, Effect effect) {
            Effect newEffect = EffectFactory.MakeEffect(effect, targetId);
            Entity entity = _entityManager.GetEntity(targetId);
            Debug.Log($"Apply Effect_{newEffect.id} -> Entity_{entity.entityId}");
            entity.AddEffect(newEffect);
        }

        public void RegisterEffect(Effect effect) {
            if (effect is IEventOn<BeforeTurnEvent> eventOn) {
                // EventBus.Register<IEventOn<BeforeTurnEvent>>(eventOn);
            }
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
            // _ReduceEffect(entityId);

            isTurnFinished = true;
        }

        private void _Trigger<T>(int entityId, T evt) {
            Entity entity = EntityManager.Instance.GetEntity(entityId);
            List<Effect> allEffects = entity.GetEffectList();

            foreach (Effect effect in allEffects) {
                if (effect is IEventOn<T> typedEffect) {
                    typedEffect.On(evt);

                    var entityObj = EntityManager.Instance.GetEntityObject(entityId);
                    EntityAnimation.PlayAnimationOnce(entityObj, PlayerState.DAMAGED);

                    if (entity.IsDead()) {
                        EntityAnimation.PlayAnimation(entityObj, PlayerState.DEATH);
                    }
                }
                else {
                    Debug.Log($"Effect {effect.GetType().Name} does NOT implement IEventOn<{typeof(T).Name}>");
                }
            }
            
            _ReduceEffect(entityId);
        }

        private void _ReduceEffect(int entityId) {
            Entity entity = EntityManager.Instance.GetEntity(entityId);
            List<Effect> allEffects = entity.GetEffectList();
            List<Effect> expiredEffects = new();

            allEffects.ForEach(effect => {
                if (effect.rounds <= 0) expiredEffects.Add(effect);
            });

            expiredEffects.ForEach(e => entity.RemoveEffect(e));
        }
    }
}
