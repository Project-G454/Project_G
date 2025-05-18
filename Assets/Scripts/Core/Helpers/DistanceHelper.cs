using System;
using UnityEngine;

namespace Core.Helpers {
    class DistanceHelper {
        public static bool InRange(Vector2 from, Vector2 to, int distance) {
            return ManhattanDistance(from, to) <= distance;
        }

        public static float ManhattanDistance(Vector2 from, Vector2 to) {
            return Mathf.RoundToInt(Math.Abs(from.x - to.x)) + Mathf.RoundToInt(Math.Abs(from.y - to.y));
        }
    }
}
