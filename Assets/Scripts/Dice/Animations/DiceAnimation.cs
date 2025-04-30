using System.Collections;
using Dice.Data;
using UnityEngine;

namespace Dice.Animations {
    public class DiceAnimation {
        public static void Roll(GameObject dice) {
            Rigidbody rb = dice.GetComponent<Rigidbody>();

            rb.linearVelocity = new Vector3(5f, 0f, 7f);
            rb.AddTorque(new Vector3(50f, 50f, 50f), ForceMode.Impulse);
        }

        public static bool IsDiceTrulyStopped(Rigidbody rb) {
            return (
                rb.linearVelocity.magnitude < 0.1f &&
                rb.angularVelocity.magnitude < 0.1f
            );
        }

        IEnumerator WaitUntilDiceStops(Rigidbody rb) {
            while (!IsDiceTrulyStopped(rb)) {
                yield return null;
            }

            // int result = DiceUtils.GetTopFace(rb.gameObject);
            // Debug.Log($"點數是：{result}");
        }
    }
}
