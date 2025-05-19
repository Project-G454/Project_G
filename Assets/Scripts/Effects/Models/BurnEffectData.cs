using UnityEngine;

namespace Effects.Data {
    [CreateAssetMenu(fileName = "Burn", menuName = "Effects/Burn")]
    class BurnEffectData: EffectData {
        public int damage;
        public override EffectType effectType => EffectType.BURN;
    }
}
