using System;
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
            float vol = Mathf.Log10(slider.value) * 20;
            targetMixer.SetFloat(mixerName, vol);
        }
    }
}
