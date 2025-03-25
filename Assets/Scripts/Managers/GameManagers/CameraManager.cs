using UnityEngine;

namespace Core.Managers {
    class CameraManager: MonoBehaviour {
        public static CameraManager Instance;
        private Camera _camera;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Init() {
            _camera = Camera.main;
        }

        void Start() {
            Init();
        }

        public void SnapCameraTo(GameObject target) {
            _camera.transform.SetParent(target.transform);
            _camera.transform.localPosition = new Vector3(0f, 0f, -10f);
        }

        public void ReleaseCamera() {
            _camera.transform.SetParent(null);
        }
    }
}
