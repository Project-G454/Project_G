using System;
using System.Collections;
using Cards.Data;
using Core.Loaders.Cards;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards.Animations {
    class ParticalAnimation {
        public static void PlayCardAnimation(GameObject target, CardAnimations animationName, Action onComplete = null) {
            GameObject effect = CardAnimationLoader.LoadCardAnimator();
            effect.transform.position = target.transform.position + new Vector3(0, 0, -1.5f);
            effect.transform.SetParent(target.transform);

            Animator animator = effect.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = CardAnimationLoader.LoadCardAnimation(animationName);
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
            CoroutineRunner.instance.StartCoroutine(AnimationCompleteCallback(
                clip.length + 0.2f,
                () => {
                    onComplete?.Invoke();
                    if (effect != null) GameObject.Destroy(effect);
                }
            ));
            
        }

        private static IEnumerator AnimationCompleteCallback(float delay, Action onComplete) {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke();
        }
    }
}
