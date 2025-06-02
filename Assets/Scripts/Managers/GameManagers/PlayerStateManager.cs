using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {
    public static PlayerStateManager Instance { get; private set; }

    public List<string> currentDeck = new List<string>(); // 可改成Card類型
    public int currentHP = 100;
    public int currentGold = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDeck(List<string> deck) {
        currentDeck = new List<string>(deck);
    }

    public void ModifyHP(int amount) {
        currentHP += amount;
    }

    public void ModifyGold(int amount) {
        currentGold += amount;
    }
}
