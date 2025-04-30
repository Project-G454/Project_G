using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dices {
    public class Dice: MonoBehaviour {
        public List<GameObject> detectors;
        public int id;

        public int GetTopFace() {
            // 透過 detector 的 y 軸高度判斷現在哪面在最上面
            int topFaceIdx = 0;
            for (int i=1; i<detectors.Count; i++) {
                if (detectors[i].transform.position.y > detectors[topFaceIdx].transform.position.y) {
                    topFaceIdx = i;
                }
            }
            return topFaceIdx+1;
        }

        public static bool IsDiceStopped(Rigidbody rb) {
            return (
                rb.linearVelocity.magnitude < 0.1f &&
                rb.angularVelocity.magnitude < 0.1f
            );
        }

        public IEnumerator WaitUntilDiceStop() {
            Rigidbody rb = GetComponent<Rigidbody>();
            while (rb && !IsDiceStopped(rb)) {
                yield return null;
            }
        }
    }
}
