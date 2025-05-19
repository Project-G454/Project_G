using UnityEngine;
using UnityEngine.UI;

namespace Effects.Data {
    public abstract class EffectData: ScriptableObject {
        public int id;
        public string effectName;
        public string effectDesc;
        public Sprite icon;
        public abstract EffectType effectType { get; }
    }
}
