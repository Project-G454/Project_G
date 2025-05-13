using Cards.Data;
using Core.Loaders.Cards;
using UnityEngine;

namespace Cards.Animations {
    class ParticalAnimation {
        public static void PlayCardAnimation(GameObject target, CardAnimations animationName) {
            GameObject effect = CardAnimationLoader.LoadCardAnimator();
            effect.transform.position = target.transform.position + new Vector3(0, 0, -1.5f);
            effect.transform.SetParent(target.transform);

            Animator animator = effect.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = CardAnimationLoader.LoadCardAnimation(animationName);
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
            // GameObject.Destroy(effect, clip.length + 0.2f);
        }
    }
}
