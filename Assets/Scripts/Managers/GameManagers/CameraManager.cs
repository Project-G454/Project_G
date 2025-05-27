using Core.Interfaces;
using UnityEngine;

namespace Core.Managers {
    public class CameraManager: MonoBehaviour, IManager {
        public static CameraManager Instance;
        private Camera _camera;

        private void Awake() {
            Instance = this;
        }

        public void Reset() {}

        public void Init() {
            _camera = Camera.main;
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
