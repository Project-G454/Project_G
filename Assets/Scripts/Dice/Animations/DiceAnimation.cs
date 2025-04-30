using System.Collections;
using Dices.Data;
using UnityEngine;

namespace Dices.Animations {
    public class DiceAnimation {
        public static void Roll(GameObject dice) {
            Rigidbody rb = dice.GetComponent<Rigidbody>();

            rb.linearVelocity = new Vector3(0f, 0f, 7f);
            rb.AddTorque(new Vector3(50f, 50f, 50f), ForceMode.Impulse);
        }
    }
}
