using UnityEngine;

namespace Effects.Data {
    [CreateAssetMenu(fileName = "Stun", menuName = "Effects/Stun")]
    class StunEffectData: EffectData {
        public override EffectType effectType => EffectType.STUN;
    }
}
