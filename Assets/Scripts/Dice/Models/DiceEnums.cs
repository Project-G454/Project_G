using System.Collections.Generic;
using UnityEngine;

namespace Dices.Data {
    public class Dice_D6 {
        public readonly static Dictionary<int, Vector3> direction = new Dictionary<int, Vector3>() {
            {1, Vector3.up},
            {2, Vector3.down},
            {3, Vector3.left},
            {4, Vector3.right},
            {5, Vector3.forward},
            {6, Vector3.back},
        };
    }
}
