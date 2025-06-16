using UnityEngine;

namespace Core.Handlers {
    /// <summary>
    /// 控制相機的跟隨、滑動和縮放功能。
    /// </summary>
    /// <remarks>
    /// 這個腳本允許相機跟隨指定的目標，並提供滑動和滾輪縮放功能。
    /// </remarks>
    [AddComponentMenu("Core/Handlers/CameraController")]
    [RequireComponent(typeof(Camera))]
    public class CameraController: MonoBehaviour {
        [Header("跟隨目標 (可選)")]
        public Transform target;
        public float followSpeed = 5f;

        [Header("滑動控制")]
        public float panSpeed = 20f;
        public bool enableDrag = true;

        [Header("滾輪縮放")]
        public float zoomSpeed = 5f;
        public float minZoom = 3f;
        public float maxZoom = 10f;

        [Header("移動邊界限制")]
        public Vector2 minBounds;
        public Vector2 maxBounds;

        private Camera cam;
        private Vector3 dragOrigin;
        public bool isFollowing = true;

        void Start() {
            cam = Camera.main;
        }

        void Update() {
            if (target != null && isFollowing) {
                Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space)) isFollowing = true;

            HandleDrag();
            HandleZoom();
            ClampPosition();
        }

        void HandleDrag() {
            if (!enableDrag) return;

            if (Input.GetMouseButtonDown(1)) {
                isFollowing = false;
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(1)) {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                transform.position += difference;
            }
        }

        void HandleZoom() {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f) {
                float newSize = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }

        void ClampPosition() {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
            transform.position = pos;
        }
    }
}

