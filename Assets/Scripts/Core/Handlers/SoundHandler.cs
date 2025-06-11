using Core.Managers.Sound;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Handlers {
    public class SoundHandler: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IBeginDragHandler, IEndDragHandler {
        public AudioClip[] hoverFX;
        public AudioClip[] clickFX;
        public AudioClip[] dragFX;
        public AudioClip[] releaseFX;
        private SoundFXManager _soundFXManager;

        public void Start() {
            _soundFXManager = SoundFXManager.Instance;
        }

        private void _Play(AudioClip audioClip, SoundType type) {
            _soundFXManager.PlaySound(audioClip, type, gameObject.transform.position);
        }

        /* ------------- UI & Raycaster ------------- */
        public void OnPointerEnter(PointerEventData eventData) => _PlayHover();
        public void OnPointerClick(PointerEventData eventData) => _PlayClick();
        public void OnBeginDrag(PointerEventData eventData) => _PlayDrag();
        public void OnEndDrag(PointerEventData eventData) => _PlayRelease();
        

        /* ------------- World Objects ------------- */
        void OnMouseEnter() { if (!_IsPointerOverUI()) _PlayHover(); }
        void OnMouseDown() { if (!_IsPointerOverUI()) _PlayClick(); }
        void OnMouseDrag() { if (!_IsPointerOverUI()) _PlayDrag(); }
        void OnMouseUp() { if (!_IsPointerOverUI()) _PlayRelease(); }

        /* ------------- Private ------------- */
        private void _PlayHover() {
            if (hoverFX.Length == 0) return;
            _Play(PickRandomFX(hoverFX), SoundType.FX);
        }

        private void _PlayClick() {
            if (clickFX.Length == 0) return;
            _Play(PickRandomFX(clickFX), SoundType.FX);
        }

        private void _PlayDrag() {
            if (dragFX.Length == 0) return;
            _Play(PickRandomFX(dragFX), SoundType.FX);
        }

        private void _PlayRelease() {
            if (releaseFX.Length == 0) return;
            _Play(PickRandomFX(releaseFX), SoundType.FX);
        }

        private AudioClip PickRandomFX(AudioClip[] clips) {
            int rng = Random.Range(0, clips.Length);
            return clips[rng];
        }

        private bool _IsPointerOverUI() {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
