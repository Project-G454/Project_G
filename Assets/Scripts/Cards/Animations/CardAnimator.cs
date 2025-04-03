using System.Collections;
using UnityEngine;

namespace Cards.Animations {
    public class CardAnimator: MonoBehaviour {
        public float duration = 0.1f;

        private RectTransform _rectTransform;
        private Coroutine _moveCoroutine;
        private Coroutine _scaleCoroutine;
        
        void Init() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void MoveTo(Vector2 targetPosition, System.Action onComplete = null) {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            Init();
            _moveCoroutine = StartCoroutine(MoveAnimation(targetPosition, onComplete));
        }

        public void ScaleTo(Vector3 targetScale, System.Action onComplete = null) {
            if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
            Init();
            _scaleCoroutine = StartCoroutine(ScaleAnimation(targetScale, onComplete));
        }

        private IEnumerator MoveAnimation(Vector2 targetPosition, System.Action onComplete = null) {
            Vector2 start = _rectTransform.anchoredPosition;
            float time = 0f;
            while (time < duration) {
                time += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, time / duration);
                _rectTransform.anchoredPosition = Vector2.Lerp(start, targetPosition, t);
                yield return null;
            }
            _rectTransform.anchoredPosition = targetPosition;
            onComplete?.Invoke();
        }

        private IEnumerator ScaleAnimation(Vector3 targetScale, System.Action onComplete = null) {
            Vector3 start = transform.localScale;
            float time = 0f;
            while (time < duration) {
                time += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, time / duration);
                transform.localScale = Vector3.Lerp(start, targetScale, t);
                yield return null;
            }
            transform.localScale = targetScale;
            onComplete?.Invoke();
        }

        public void StopAllAnimations() {
            StopCoroutine(_moveCoroutine);
            StopCoroutine(_scaleCoroutine);
        }
    }
}
