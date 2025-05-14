using Core.Entities;
using Effects.Data;
using Entities;
using UnityEngine;

namespace Effects {
    public class Effect {
        private EffectData _effectData;
        public int id { get => _effectData.id; }
        public string name { get => _effectData.name; }
        public string effectDesc { get => effectDesc; }
        public int rounds { get; set; }
        public Sprite icon { get; set; }
        public EffectType effectType { get; set; }
        public int behaviourId { get; set; }

        public Effect(EffectData effectData, int behaviourId) {
            this._effectData = effectData;
            this.rounds = effectData.rounds;
            this.icon = effectData.icon;
            this.effectType = effectData.effectType;
            this.behaviourId = behaviourId;
        }

        public virtual void Trigger() {
            Debug.Log($"Effect_{id} -> Entity_{behaviourId}");
        }
    }
}
