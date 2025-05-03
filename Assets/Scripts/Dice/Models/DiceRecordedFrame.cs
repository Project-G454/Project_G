using System.Collections.Generic;
using UnityEngine;


namespace Dices.Data {
    public class DiceRecordedFrame {
        public Vector3 position;
        public Quaternion rotation;
        public bool isStopped;

        public DiceRecordedFrame(
            Vector3 position,
            Quaternion rotation,
            bool isStopped
        ) {
            this.position = position;
            this.rotation = rotation;
            this.isStopped = isStopped;
        }
    }

    public class DiceRecord {
        public GameObject dice;
        public List<DiceRecordedFrame> frames;
        public DiceRecord(GameObject dice) {
            this.dice = dice;
            this.frames = new();
        }
    }
}
