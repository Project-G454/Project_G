using UnityEngine;

namespace Effects.Data {
    [CreateAssetMenu(fileName = "Poison", menuName = "Effects/Poison")]
    class PoisonEffectData: EffectData {
        public int damage;
        public override EffectType effectType => EffectType.POISON;
    }
}
