using System;
using UnityEngine;

namespace Agents.Helpers {
    class DistanceHelper {
        public static bool InRange(Vector2 from, Vector2 to, int distance) {
            return Vector2.Distance(from, to) <= distance;
        }
    }
}
