using System;
using Core.Entities;
using Entities;
using UnityEngine;

namespace Agents.Helpers {
    class DistanceHelper {
        public static bool InRange(Vector2 from, Vector2 to, int distance) {
            return ManhattanDistance(from, to) <= distance;
        }

        public static float ManhattanDistance(Vector2 from, Vector2 to) {
            return Mathf.RoundToInt(Math.Abs(from.x - to.x)) + Mathf.RoundToInt(Math.Abs(from.y - to.y));
        }

        public static bool EntityInRange(Entity from, Entity to, int range) {
            GameObject sourceObj = EntityManager.Instance.GetEntityObject(from.entityId);
            return EntityInRange(sourceObj, to, range);
        }

        public static bool EntityInRange(GameObject from, Entity to, int range) {
            GameObject targetObj = EntityManager.Instance.GetEntityObject(to.entityId);
            return DistanceHelper.InRange(from.transform.position, targetObj.transform.position, range) && !to.IsDead();
        }

        public static bool EntityInRange(GameObject from, GameObject to, int range) {
            return DistanceHelper.InRange(from.transform.position, to.transform.position, range);
        }
    }
}
