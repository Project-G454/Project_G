using System.Collections;
using System.Collections.Generic;
using Core.Loaders.Dices;
using Dices.Data;
using UnityEngine;

namespace Dices {
    public class Dice: MonoBehaviour {
        public List<GameObject> detectors;
        public GameObject inner;
        public int id;
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public Vector3 initialScale;

        public void OnCollisionEnter(Collision collision) {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && collision.gameObject.CompareTag("Bouncing Wall")) {
                rb.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
                // rb.AddTorque(new Vector3(5f, 5f, 5f), ForceMode.Impulse);
            }
        }

        public int GetTopFace() {
            // 透過 detector 的 y 軸高度判斷現在哪面在最上面
            int topFaceIdx = 0;
            for (int i = 1; i < detectors.Count; i++) {
                if (detectors[i].transform.position.y > detectors[topFaceIdx].transform.position.y) {
                    topFaceIdx = i;
                }
            }
            return topFaceIdx + 1;
        }

        public bool IsDiceStopped() {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb == null) return true;

            return rb.linearVelocity.magnitude < 0.05f && rb.angularVelocity.magnitude < 0.05f;
        }

        public void RotateFace(int from, int to) {
            DiceD6Rotation rotationData = DiceDataLoader.LoadD6Rotation();
            Vector3 degree = rotationData.relativeRotation[from - 1].rotation[to - 1].rotation;

            Transform innerT = inner.GetComponent<Transform>();
            if (innerT) {
                innerT.Rotate(degree.x, degree.y, degree.z, Space.Self);
            }
        }

        public void ResetToInitialState() {
            if (initialPosition == null) return;
            if (initialRotation == null) return;
            if (initialScale == null) return;
            transform.SetPositionAndRotation(initialPosition, initialRotation);
            transform.localScale = initialScale;
        }

        public void CaptureTransform() {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialScale = transform.localScale;
        }

        public void DisablePhysics() {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        public void EnablePhysics() {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        public void Hidden() {
            MeshRenderer renderer = inner.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;
        }

        public void Show() {
            MeshRenderer renderer = inner.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = true;
        }
    }
}
