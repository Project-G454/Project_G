using System.Collections;
using Dices.Data;
using UnityEngine;

namespace Dices.Animations {
    public class DiceAnimation {
        public static void Roll(GameObject dice) {
            Rigidbody rb = dice.GetComponent<Rigidbody>();

            rb.linearVelocity = new Vector3(Random.Range(5f, -5f), -10f, Random.Range(5f, 15f));
            rb.AddTorque(new Vector3(Random.Range(50f, 30f), Random.Range(50f, 30f), Random.Range(50f, 30f)), ForceMode.Impulse);
        }
    }
}
