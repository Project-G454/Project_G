using System.Collections;
using UnityEngine;

namespace Entities.Animations {
    public class EntityAnimation {
        public static void PlayAnimation(GameObject entityObj, PlayerState state) {
            PlayerObj motionHandler = entityObj.GetComponentInChildren<PlayerObj>();
            motionHandler.SetState(state);
        }

        public static void PlayAnimationOnce(GameObject entityObj, PlayerState state) {
            PlayerObj motionHandler = entityObj.GetComponentInChildren<PlayerObj>();
            motionHandler.PlayStateAnimation(state);
        }
    }
}
