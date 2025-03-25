using Effects.Data;
using Systems.Interactions;
using UnityEngine;

namespace Effects.Categories {
    class PoisonEffect: Effect {
        private int _damage;
        public PoisonEffect(
            PoisonEffectData effectData
        ): base(effectData) {
            this._damage = effectData.damage;
            this.effectType = EffectType.POISON;
        }

        public override void Trigger(int targetId) {
            base.Trigger(targetId);
            EffectPlayerInteraction.ApplyDamage(targetId, _damage);
            Debug.Log($"PoisonEffect -> Entity_{targetId}");
        }
    }
}
