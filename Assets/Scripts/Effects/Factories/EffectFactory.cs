using Effects.Categories;
using Effects.Data;

namespace Effects.Factories {
    public class EffectFactory {
        public static Effect MakeEffect(EffectData effectData) {
            Effect effect;
            switch (effectData.effectType) {
                case EffectType.POISON:
                    effect = new PoisonEffect(effectData as PoisonEffectData);
                    break;
                default:
                    effect = new Effect(effectData);
                    break;
            }
            return effect;
        }

        public static EffectData GetFakeEffect() {
            EffectData effectData = new PoisonEffectData();
            effectData.effectId = 0;
            effectData.effectName = "Poison";
            effectData.effectDesc = "poison";
            effectData.effectType = EffectType.POISON;
            effectData.rounds = 5;
            return effectData;
        }
    }
}
