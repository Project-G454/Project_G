using Core.Interfaces;
using Dice.Animations;
using UnityEngine;

namespace Core.Managers.Dice {
    public class DiceManager: MonoBehaviour, IManager {
        public static DiceManager Instance;
        public GameObject dicePrefab;
        public Transform diceParent;

        void IManager.Init() {}

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int Roll(int min, int max) {
            int point = Random.Range(min, max);

            GameObject dice = CreateDice();
            DiceAnimation.Roll(dice);
            
            // StartCoroutine(DiceAnimation.ForceDiceResult(dice, point));
            return point;
        }

        public GameObject CreateDice() {
            GameObject dice = Instantiate(dicePrefab, diceParent.position, Quaternion.identity, diceParent);
            Transform diceT = dice.GetComponent<Transform>();
            if (diceT) {
                Vector3 offset = new Vector3(Random.Range(0, 10), 0f, Random.Range(0, 10));
                diceT.SetParent(diceParent);
                diceT.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                diceT.position += offset;
            }
            return dice;
        }
    }
}
