using Core.Interfaces;
using Effects.Data;
using Events;
using Systems.Interactions;
using UnityEngine;

namespace Effects.Categories {
    class StunEffect: Effect, IEventOn<BeforeTurnEvent> {
        public StunEffect(
            StunEffectData effectData,
            int behaviourId,
            int rounds
        ) : base(effectData, behaviourId, rounds) {

        }

        public override void Trigger() {
            base.Trigger();
            Debug.Log($"StunEffect -> Entity_{this.behaviourId}");
        }

        void IEventOn<BeforeTurnEvent>.On(BeforeTurnEvent triggerEvent) {
            Trigger();
        }

        public override Effect Copy() {
            return new StunEffect((StunEffectData)effectData, behaviourId, rounds);
        }
    }
}
