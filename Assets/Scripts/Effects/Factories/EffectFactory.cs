using Effects.Categories;
using Effects.Data;

namespace Effects.Factories {
    public class EffectFactory {
        public static Effect MakeEffect(EffectData effectData, int behaviourId) {
            Effect effect;
            switch (effectData.effectType) {
                case EffectType.POISON:
                    effect = new PoisonEffect(effectData as PoisonEffectData, behaviourId);
                    break;
                default:
                    effect = new Effect(effectData, behaviourId);
                    break;
            }
            return effect;
        }
    }
}
