using UnityEngine;

namespace Effects.Data {
    public abstract class EffectData: ScriptableObject {
        public int id;
        public string effectName;
        public string effectDesc;
        public int rounds;
        public abstract EffectType effectType { get; }
    }
}
