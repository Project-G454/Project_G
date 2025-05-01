using System.Collections;
using System.Collections.Generic;
using Core.Interfaces;
using Dices;
using Dices.Animations;
using Dices.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Managers.Dices {
    public class DiceManager: MonoBehaviour, IManager {
        public static DiceManager Instance;
        public GameObject dicePrefab;
        public Transform diceParent;
        public Transform generatePoint;
        private List<GameObject> dices = new();

        void IManager.Init() {}

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public List<int> Roll(int min, int max, int count=1, List<int> forcePoints=null) {
            StopAllCoroutines();
            ResetDiceObjects();

            // Roll dices
            List<int> points = new();
            if (forcePoints != null) {
                foreach(int point in forcePoints) {
                    points.Add(point);
                }
            }

            count -= points.Count;
            for(int i=0; i<count; i++) {
                GameObject diceObj = CreateDice(i);
                this.dices.Add(diceObj);
                
                if (forcePoints == null) {
                    int point = Random.Range(min, max);
                    points.Add(point);
                }
            }
            StartCoroutine(SimulateRoll(this.dices, points));               
            Debug.Log($"Dices: {points}");
            return points;
        }

        public void ResetDiceObjects() {
            if (this.dices == null || this.dices.Count == 0) return;

            List<GameObject> diceObjCopy = new List<GameObject>(this.dices);
            foreach (GameObject diceObj in diceObjCopy) {
                if (diceObj != null) {
                    RemoveDice(diceObj);
                }
            }
        }

        public void RemoveDice(GameObject diceObj) {
            if (diceObj != null && dices.Contains(diceObj)) {
                dices.Remove(diceObj);
                Destroy(diceObj);
            }
        }

        public IEnumerator GetRollResult(Dice dice) {
            yield return WaitUntilDiceStop(dice);
            int point = dice.GetTopFace();
            Debug.Log($"Dice_{dice.id} Point: {point}");
        }

        public IEnumerator WaitUntilDiceStop(Dice dice) {
            while (!dice.IsDiceStopped()) {
                yield return null;
            }
        }

        public GameObject CreateDice(int diceId) {
            GameObject diceObj = Instantiate(dicePrefab, generatePoint.position, Quaternion.identity, diceParent);

            Dice dice = diceObj.GetComponent<Dice>();
            dice.id = diceId;

            Transform diceT = diceObj.GetComponent<Transform>();
            if (diceT) {
                Vector3 offset = new Vector3(Random.Range(-2, 2), diceId * 1.2f, Random.Range(-2, 2));
                diceT.SetParent(diceParent);
                diceT.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                diceT.position += offset;
                dice.CaptureTransform();
            }
            
            return diceObj;
        }

        public IEnumerator SimulateRoll(List<GameObject> dices, List<int> targetPoints) {
            yield return null; // 等待一幀，等 Destory 生效

            foreach (GameObject diceObj in dices) {
                Dice dice = diceObj.GetComponent<Dice>();
                dice.Hidden();
                DiceAnimation.Roll(diceObj); 
            }
            
            Physics.simulationMode = SimulationMode.Script;
            List<DiceRecord> records = new();
            for (int frame=0; frame<500; frame++) {
                for (int i=0; i<dices.Count; i++) {
                    if (dices[i] == null || dices[i].GetComponent<Dice>() == null) continue;
                    Dice dice = dices[i].GetComponent<Dice>();
                    Rigidbody rb = dice.GetComponent<Rigidbody>();
                    
                    Vector3 position = rb.position;
                    Quaternion rotation = rb.rotation;
                    DiceRecordedFrame recordedFrame = new DiceRecordedFrame(position, rotation, dice.IsDiceStopped());
                    
                    if (frame == 0) records.Add(new DiceRecord(dices[i]));
                    records[i].frames.Add(recordedFrame);
                }
                Physics.Simulate(Time.fixedDeltaTime);
            }
            Physics.simulationMode = SimulationMode.FixedUpdate;

            for (int i=0; i<dices.Count; i++) {

                Dice dice = dices[i].GetComponent<Dice>();
                dice.DisablePhysics();
                
                int point = dice.GetTopFace();
                dice.RevertToInitialState();
                dice.RotateFace(point, targetPoints[i]);
                dice.Show();
            }

            yield return null; // 等待一幀，等 Simulate 完全結束
            foreach (DiceRecord record in records) {
                StartCoroutine(PlayDiceRecordFrames(record));
            }
        }

        public IEnumerator PlayDiceRecordFrames(DiceRecord record) {
            GameObject diceObj = record.dice;
            Dice dice = diceObj.GetComponent<Dice>();
            
            foreach (DiceRecordedFrame frame in record.frames) {
                diceObj.transform.SetPositionAndRotation(frame.position, frame.rotation);
                yield return new WaitForFixedUpdate();
            }
            dice.EnablePhysics();
        }
    }
}
