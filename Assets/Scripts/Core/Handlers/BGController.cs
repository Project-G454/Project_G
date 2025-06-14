using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Handlers {
    public class BGController: MonoBehaviour {
        private Vector3 _oriPos;
        public float parallaxIntensity = 20f;
        public float moveSpeed = 3f;

        void Start() {
            _oriPos = transform.position;
        }

        void Update() {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

            // 將螢幕座標轉為 viewport (0~1)，再平移至 -0.5~0.5 範圍
            Vector2 mouseNormalized = new Vector2(
                (mouseScreenPos.x / Screen.width) - 0.5f,
                (mouseScreenPos.y / Screen.height) - 0.5f
            );

            // 計算目標位置：根據滑鼠偏移原始位置
            Vector3 targetPosition = _oriPos + new Vector3(
                mouseNormalized.x * parallaxIntensity,
                mouseNormalized.y * parallaxIntensity,
                0
            );

            // 平滑移動
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}
