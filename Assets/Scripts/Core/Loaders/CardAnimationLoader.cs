using Cards.Data;
using UnityEngine;

namespace Core.Loaders.Cards {
    class CardAnimationLoader {
        public static GameObject LoadCardAnimator() {
            GameObject animator = GameObject.Instantiate(Resources.Load<GameObject>("Cards/Animators/AnimationContainer"));
            return animator;
        }

        public static RuntimeAnimatorController LoadCardAnimation(CardAnimations name) {
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Cards/Animators/Partical/" + name.ToString());
            return controller;
        }
    }
}
