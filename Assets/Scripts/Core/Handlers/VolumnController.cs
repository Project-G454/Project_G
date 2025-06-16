using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Core.Handlers {
    public class VolumnController: MonoBehaviour {
        public AudioMixer targetMixer;
        public Slider slider;

        public void SetMasterVol() => _SetVolumn("Master");
        public void SetMusicVol() => _SetVolumn("Music");
        public void SetFXVol() => _SetVolumn("FX");

        private void _SetVolumn(string mixerName) {
            float vol = slider.value;
            targetMixer.SetFloat(mixerName, vol);
        }
    }
}
