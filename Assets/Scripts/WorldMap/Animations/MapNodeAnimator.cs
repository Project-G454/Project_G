using DG.Tweening;
using UnityEngine;

namespace WorldMap.Animations {
    public class MapNodeAnimator: MonoBehaviour {
        public GameObject iconObj;
        private Tween _isShaking;
        private Tween _isBlinking;
        private Transform _iconTransform;
        private SpriteRenderer _iconSR;

        void Start() {
            _iconTransform = iconObj.transform;
            _iconSR = iconObj.GetComponent<SpriteRenderer>();
        }

        public void Shake(float duration = 0.5f) {
            if (_iconTransform == null) return;
            if (_isShaking != null) {
                _isShaking.Kill();
                _iconTransform.localPosition = Vector2.zero;
            }
            _isShaking = _iconTransform.DOShakePosition(duration);
        }

        public void Blink(Color ori, float duration = 0.5f) {
            if (_iconSR == null) return;
            if (_isBlinking != null) {
                _isBlinking.Kill();
            }
            _iconSR.color = Color.red;
            _isBlinking = _iconSR.DOColor(ori, duration);
        }
    }
}
