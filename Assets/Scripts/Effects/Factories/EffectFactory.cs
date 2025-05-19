using Effects.Categories;
using Effects.Data;

namespace Effects.Factories {
    public class EffectFactory {
        public static Effect MakeEffect(Effect effect, int targetId) {
            Effect newEffect;
            switch (effect.effectType) {
                case EffectType.POISON:
                    newEffect = new PoisonEffect(effect.effectData as PoisonEffectData, targetId, effect.rounds);
                    break;
                case EffectType.BURN:
                    newEffect = new BurnEffect(effect.effectData as BurnEffectData, targetId, effect.rounds);
                    break;
                case EffectType.STUN:
                    newEffect = new StunEffect(effect.effectData as StunEffectData, targetId, effect.rounds);
                    break;
                default:
                    newEffect = new Effect(effect.effectData, targetId, effect.rounds);
                    break;
            }
            return newEffect;
        }
    }
}
