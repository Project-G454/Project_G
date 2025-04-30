using System.Collections;
using System.Collections.Generic;
using Core.Interfaces;
using Dices;
using Dices.Animations;
using UnityEngine;

namespace Core.Managers.Dices {
    public class DiceManager: MonoBehaviour, IManager {
        public static DiceManager Instance;
        public GameObject dicePrefab;
        public Transform diceParent;
        public Transform generatePoint;

        void IManager.Init() {}

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int Roll(int min, int max, int count=1) {
            int point = Random.Range(min, max);
            List<GameObject> dices = new();

            // Roll dices
            for(int i=0; i<count; i++) {
                GameObject diceObj = CreateDice(i);
                Dice dice = diceObj.GetComponent<Dice>();
                DiceAnimation.Roll(diceObj);
                dices.Add(diceObj);

                StartCoroutine(GetRollResult(dice));
            }
            
            return point;
        }

        public IEnumerator GetRollResult(Dice dice) {
            yield return dice.WaitUntilDiceStop();
            int point = dice.GetTopFace();
            Debug.Log($"Dice_{dice.id} Point: {point}");
        }
        public GameObject CreateDice(int diceId) {
            GameObject diceObj = Instantiate(dicePrefab, generatePoint.position, Quaternion.identity, diceParent);

            Dice dice = diceObj.GetComponent<Dice>();
            dice.id = diceId;

            Transform diceT = diceObj.GetComponent<Transform>();
            if (diceT) {
                Vector3 offset = new Vector3(Random.Range(-2, 2), 0f, Random.Range(-2, 2));
                diceT.SetParent(diceParent);
                diceT.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                diceT.position += offset;
            }
            
            return diceObj;
        }
    }
}
