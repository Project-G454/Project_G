using System.Collections;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Managers.Sound {
    public enum SoundType {
        MASTER,
        FX,
        MUSIC
    }

    public class SoundFXManager: MonoBehaviour, IManager {
        public static SoundFXManager Instance;
        public GameObject soundPlayer; // Prefab
        public Transform container; // Parent
        public AudioMixer audioMixer;

        public void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(container);
        }
        
        public void Init() { }
        public void Reset() { }

        public void PlaySound(AudioClip audio, SoundType type = SoundType.MASTER, Vector3? playAt = null) {
            GameObject newSoundPlayer = Instantiate(soundPlayer, container);
            DontDestroyOnLoad(newSoundPlayer);
            
            AudioSource source = newSoundPlayer.GetComponent<AudioSource>();
            source.resource = audio;
            source.outputAudioMixerGroup = type switch {
                SoundType.MASTER => audioMixer.FindMatchingGroups("Master")[0],
                SoundType.FX => audioMixer.FindMatchingGroups("Master/Sound FX")[0],
                SoundType.MUSIC => audioMixer.FindMatchingGroups("Master/Music")[0],
                _ => audioMixer.FindMatchingGroups("Master")[0]
            };
            source.transform.position = playAt ?? Vector3.zero;
            StartCoroutine(_Play(source));
        }

        private IEnumerator _Play(AudioSource source) {
            GameObject playerObj = source.gameObject;
            source.Play();
            yield return new WaitUntil(() => !source.isPlaying);
            Destroy(playerObj);
        }
    }
}
