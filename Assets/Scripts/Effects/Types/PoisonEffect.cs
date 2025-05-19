using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Effects.Data;
using Entities;
using Events;
using Systems.Interactions;
using Unity.VisualScripting;
using UnityEngine;

namespace Effects.Categories {
    class PoisonEffect: Effect, IEventOn<BeforeTurnEvent> {
        public PoisonEffect(
            PoisonEffectData effectData,
            int behaviourId,
            int rounds
        ) : base(effectData, behaviourId, rounds) {}

        public override void Init() {
            base.Init();
        }

        public override void Trigger() {
            Entity target = EntityManager.Instance.GetEntity(behaviourId);
            List<Effect> effects = target.GetEffectList();
            Effect existing = effects.FirstOrDefault(e => e.id == id);
            EffectPlayerInteraction.ApplyDamage(this.behaviourId, existing.rounds);
            Debug.Log($"PoisonEffect -> Entity_{this.behaviourId}");
            base.Trigger();
        }

        void IEventOn<BeforeTurnEvent>.On(BeforeTurnEvent triggerEvent) {
            Trigger();
        }
        
        public override Effect Copy() {
            return new PoisonEffect((PoisonEffectData)effectData, behaviourId, rounds);
        }
    }
}
