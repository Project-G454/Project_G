using System.Collections.Generic;
using UnityEngine;
using Core.Game;
using Entities;

public class PlayerStateManager : MonoBehaviour {
    public static PlayerStateManager Instance { get; private set; }

    [SerializeField] private List<GamePlayerState> allPlayers = new List<GamePlayerState>();
    private Dictionary<int, GamePlayerState> players = new Dictionary<int, GamePlayerState>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(int id, string name, EntityClasses entityClass, int gold = 0) {
        if (!players.ContainsKey(id)) {
            var state = new GamePlayerState(id, name, entityClass, gold);
            players[id] = state;
            allPlayers.Add(state);
        }
    }

    public GamePlayerState GetPlayer(int id) {
        return players.ContainsKey(id) ? players[id] : null;
    }

    public List<GamePlayerState> GetAllPlayer() {
        return allPlayers;
    }

    public void ModifyHP(int id, int delta) {
        if (players.ContainsKey(id)) {
            players[id].hp += delta;
        }
    }

    public void ModifyGold(int id, int delta) {
        if (players.ContainsKey(id)) {
            players[id].gold += delta;
        }
    }

    public void SetDeck(int id, List<int> newDeck) {
        if (players.ContainsKey(id)) {
            players[id].deck = new List<int>(newDeck);
        }
    }

    public void ClearAllPlayers() {
        players.Clear();
        allPlayers.Clear();
    }
}
