using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Dices;
using Dices.Animations;
using Dices.Data;
using UnityEngine;

namespace Core.Managers.Dices {
    /// <summary>
    /// 擲骰管理器，處理骰子的建立、模擬與結果控制。
    /// </summary>
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

        /// <summary>
        /// 擲出骰子，返回對應的點數清單。
        /// </summary>
        /// <param name="min">最小點數（限制範圍為 1～6）。</param>
        /// <param name="max">最大點數（限制範圍為 1～6）。</param>
        /// <param name="count">要擲出的骰子數量。</param>
        /// <param name="forcePoints">可選的強制點數清單，用於部分骰子。</param>
        /// <returns>點數結果清單。</returns>
        public List<int> Roll(int min, int max, int count=1, List<int> forcePoints=null) {
            const int MINIMUM = 1;
            const int MAXIMUM = 6;
            min = Math.Clamp(min, MINIMUM, MAXIMUM);
            max = Math.Clamp(max, MINIMUM, MAXIMUM);

            StopAllCoroutines();
            ResetDiceObjects();

            List<int> points = new();
            points.AddRange(forcePoints ?? Enumerable.Empty<int>());

            // 建立骰子並決定點數
            count -= points.Count;
            for(int i=0; i<count; i++) {
                GameObject diceObj = CreateDice(i);
                this.dices.Add(diceObj);
                int point = UnityEngine.Random.Range(min, max);
                points.Add(point);
            }

            // Roll dices
            StartCoroutine(_SimulateRoll(this.dices, points));               
            Debug.Log($"Dices: {string.Join(", ", points)}");
            return points;
        }

        /// <summary>
        /// 重設場上的所有骰子，將它們從場景中移除並銷毀。
        /// </summary>
        public void ResetDiceObjects() {
            if (this.dices == null || this.dices.Count == 0) return;

            List<GameObject> diceObjCopy = new List<GameObject>(this.dices);
            foreach (GameObject diceObj in diceObjCopy) {
                if (diceObj != null) {
                    RemoveDice(diceObj);
                }
            }
        }

        /// <summary>
        /// 移除指定的骰子物件並將其從管理清單中刪除。
        /// </summary>
        public void RemoveDice(GameObject diceObj) {
            if (diceObj != null && dices.Contains(diceObj)) {
                dices.Remove(diceObj);
                Destroy(diceObj);
            }
        }

        /// <summary>
        /// 建立單顆骰子物件，設定其位置與初始旋轉狀態。
        /// </summary>
        /// <param name="diceId">此骰子的編號，用來分散初始位置。</param>
        /// <returns>建立好的骰子 GameObject。</returns>
        public GameObject CreateDice(int diceId) {
            GameObject diceObj = Instantiate(dicePrefab, generatePoint.position, Quaternion.identity, diceParent);

            Dice dice = diceObj.GetComponent<Dice>();
            dice.id = diceId;

            Transform diceT = diceObj.GetComponent<Transform>();
            if (diceT) {
                diceT.SetParent(diceParent);
                Vector3 positionOffset = new Vector3(
                    UnityEngine.Random.Range(-2, 2), 
                    diceId * 1.2f, // 避免骰子重疊
                    UnityEngine.Random.Range(-2, 2)
                );
                Vector3 rotationOffset = new Vector3(
                    UnityEngine.Random.Range(0, 360), 
                    UnityEngine.Random.Range(0, 360), 
                    UnityEngine.Random.Range(0, 360)
                );
                diceT.rotation = Quaternion.Euler(rotationOffset);
                diceT.position += positionOffset;
                dice.CaptureTransform(); // 紀錄建立時的初始狀態
            }
            
            return diceObj;
        }

        /// <summary>
        /// 執行骰子模擬與動畫過程：
        /// <para>1. 預模擬 500 幀取得軌跡與最終點數。</para>
        /// <para>2. 設定為目標點數，進行實際動畫播放。</para>
        /// </summary>
        /// <param name="dices">要模擬的骰子清單。</param>
        /// <param name="targetPoints">目標點數清單。</param>
        /// <returns>IEnumerator 協程。</returns>
        private IEnumerator _SimulateRoll(List<GameObject> dices, List<int> targetPoints) {
            yield return null; // 等待 1 幀，等 Destory 生效
            const int SIMULATION_STEPS = 500; // (模擬 500 幀)

            // Roll 第一次 (模擬用)
            foreach (GameObject diceObj in dices) {
                Dice dice = diceObj.GetComponent<Dice>();
                dice.Hidden();
                DiceAnimation.Roll(diceObj); 
            }
            
            // 在 1 幀內計算第一次 Roll 的結果
            Physics.simulationMode = SimulationMode.Script; // 開始模擬
            List<DiceRecord> records = new();
            for (int frame=0; frame<SIMULATION_STEPS; frame++) {
                // 將每個骰子的路徑逐幀記錄下來
                for (int i=0; i<dices.Count; i++) {
                    if (dices[i] == null) continue; // 如果骰子已被刪除就跳過
                    DiceRecordedFrame recordedFrame = DiceAnimation.RecordFrame(dices[i]);
                    if (frame == 0) records.Add(new DiceRecord(dices[i]));
                    if (recordedFrame != null) records[i].frames.Add(recordedFrame);
                }
                Physics.Simulate(Time.fixedDeltaTime); // 模擬 1 幀
            }
            Physics.simulationMode = SimulationMode.FixedUpdate; // 結束模擬

            // 禁用骰子所有的物理計算，並回歸初始狀態
            for (int i=0; i<dices.Count; i++) {
                Dice dice = dices[i].GetComponent<Dice>();
                dice.DisablePhysics();
                
                int point = dice.GetTopFace();
                dice.ResetToInitialState();
                dice.RotateFace(point, targetPoints[i]);
                dice.Show();
            }

            yield return null; // 等待一幀，等 Simulate 完全結束

            // 播放剛才模擬時儲存的位置、旋轉角度資訊 (第 2 次 Roll)
            foreach (DiceRecord record in records) {
                StartCoroutine(DiceAnimation.PlayDiceRecordFrames(record));
            }
        }
    }
}
