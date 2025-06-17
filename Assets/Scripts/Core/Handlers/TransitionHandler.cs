using System.Collections;
using UnityEngine;

namespace Core.Handlers {
    public class TransitionHandler: MonoBehaviour {
        public RuntimeAnimatorController anim;
        public Animator _animator;
        public static TransitionHandler Instance;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator StartTransition() {
            if (_animator == null) yield break;
            _animator.runtimeAnimatorController = anim;
            _animator.SetTrigger("Start");
            yield return new WaitForSeconds(anim.animationClips[0].length + 0.2f);
        }

        public IEnumerator EndTransition() {
            if (_animator == null) yield break;
            _animator.runtimeAnimatorController = anim;
            _animator.SetTrigger("End");
            yield return new WaitForSeconds(anim.animationClips[0].length + 0.2f);
        }
    }
}
