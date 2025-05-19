using Core.Interfaces;
using Effects.Data;
using Events;
using Systems.Interactions;
using Unity.VisualScripting;
using UnityEngine;

namespace Effects.Categories {
    class BurnEffect: Effect, IEventOn<BeforeTurnEvent> {
        private int _damage;

        public BurnEffect(
            BurnEffectData effectData,
            int behaviourId,
            int rounds
        ) : base(effectData, behaviourId, rounds) {
            this._damage = effectData.damage;
        }

        public override void Init() {
            base.Init();
            if (effectData is BurnEffectData poisonData) {
                this._damage = poisonData.damage;
            }
        }

        public override void Trigger() {
            base.Trigger();
            EffectPlayerInteraction.ApplyDamage(this.behaviourId, _damage);
            Debug.Log($"BurnEffect -> Entity_{this.behaviourId}");
        }

        void IEventOn<BeforeTurnEvent>.On(BeforeTurnEvent triggerEvent) {
            Trigger();
        }
        
        public override Effect Copy() {
            return new BurnEffect((BurnEffectData)effectData, behaviourId, rounds);
        }
    }
}
