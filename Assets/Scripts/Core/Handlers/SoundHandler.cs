using Core.Managers.Sound;
using UnityEngine;

namespace Core.Handlers {
    public class SoundHandler: MonoBehaviour {
        public AudioClip audioClip;
        public SoundType type;
        private SoundFXManager _soundFXManager;

        public void Start() {
            _soundFXManager = SoundFXManager.Instance;
        }

        public void Play() {
            _soundFXManager.PlaySound(audioClip, type, gameObject.transform.position);
        }
    }
}
