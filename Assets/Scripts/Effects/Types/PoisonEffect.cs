using Core.Interfaces;
using Effects.Data;
using Events;
using Systems.Interactions;
using UnityEngine;

namespace Effects.Categories {
    class PoisonEffect: Effect, IEventOn<BeforeTurnEvent> {
        private int _damage;
        public PoisonEffect(
            PoisonEffectData effectData,
            int behaviourId
        ): base(effectData, behaviourId) {
            this._damage = effectData.damage;
        }

        public override void Trigger() {
            if (this.rounds <= 0) return;
            base.Trigger();
            EffectPlayerInteraction.ApplyDamage(this.behaviourId, _damage);
            Debug.Log($"PoisonEffect -> Entity_{this.behaviourId}");
        }

        void IEventOn<BeforeTurnEvent>.On(BeforeTurnEvent triggerEvent) {
            Trigger();
        }
    }
}
