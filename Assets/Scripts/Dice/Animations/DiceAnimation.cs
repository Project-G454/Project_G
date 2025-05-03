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

        public static IEnumerator PlayDiceRecordFrames(DiceRecord record) {
            GameObject diceObj = record.dice;
            Dice dice = diceObj.GetComponent<Dice>();
            
            foreach (DiceRecordedFrame frame in record.frames) {
                diceObj.transform.SetPositionAndRotation(frame.position, frame.rotation);
                yield return new WaitForFixedUpdate();
            }
            dice.EnablePhysics();
        }

        public static DiceRecordedFrame RecordFrame(GameObject diceObj) {
            if (diceObj.GetComponent<Dice>() == null) return null;
            if (diceObj.GetComponent<Rigidbody>() == null) return null;
            Dice dice = diceObj.GetComponent<Dice>();
            Rigidbody rb = diceObj.GetComponent<Rigidbody>();
            
            // 紀錄 Rigidbody 的位置及旋轉角度
            // 不能紀錄 Transform 的資訊，否則有可能導致骰子錯位
            // (之後可以再儲存碰撞時間，用於播放音效)
            Vector3 position = rb.position;
            Quaternion rotation = rb.rotation;
            DiceRecordedFrame recordedFrame = new DiceRecordedFrame(position, rotation, dice.IsDiceStopped());
            return recordedFrame;
        }
    }
}
