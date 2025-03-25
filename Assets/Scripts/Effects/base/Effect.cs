using Core.Entities;
using Effects.Data;
using Entities;
using UnityEngine;

namespace Effects {
    public class Effect {
        private EffectData _effectData;
        public int effectId { get => _effectData.effectId; }
        public string effectName { get => _effectData.effectName; }
        public string effectDesc { get => effectDesc; }
        public int rounds { get; set; }
        public EffectType effectType { get; set; }

        public Effect(EffectData effectData) {
            this._effectData = effectData;
            this.rounds = effectData.rounds;
            this.effectType = effectData.effectType;
        }

        public virtual void Trigger(int targetId) {
            this.rounds--;
            Debug.Log($"Effect_{effectId} -> Entity_{targetId}");
        }
    }
}
