using System.Collections;
using UnityEngine;

namespace Core.Handlers {
    public class TransitionHandler: MonoBehaviour {
        public RuntimeAnimatorController anim;
        public Animator _animator;

        void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator StartTransition() {
            if (_animator == null) yield break;
            _animator.runtimeAnimatorController = anim;
            _animator.SetTrigger("Start");
            yield return new WaitForSeconds(anim.animationClips[0].length);
        }

        public IEnumerator EndTransition() {
            if (_animator == null) yield break;
            _animator.runtimeAnimatorController = anim;
            _animator.SetTrigger("End");
            yield return new WaitForSeconds(anim.animationClips[0].length);
        }
    }
}
