using System.Collections.Generic;
using UnityEngine;
using Core.Game;

public class PlayerStateManager : MonoBehaviour {
    public static PlayerStateManager Instance { get; private set; }

    [SerializeField] private List<GamePlayerState> allPlayers = new List<GamePlayerState>();
    private Dictionary<string, GamePlayerState> players = new Dictionary<string, GamePlayerState>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(string id, List<string> deck, int hp = 100, int gold = 0) {
        if (!players.ContainsKey(id)) {
            var state = new GamePlayerState(id, deck, hp, gold);
            players[id] = state;
            allPlayers.Add(state);
        }
    }

    public GamePlayerState GetPlayer(string id) {
        return players.ContainsKey(id) ? players[id] : null;
    }

    public void ModifyHP(string id, int delta) {
        if (players.ContainsKey(id)) {
            players[id].hp += delta;
        }
    }

    public void ModifyGold(string id, int delta) {
        if (players.ContainsKey(id)) {
            players[id].gold += delta;
        }
    }

    public void SetDeck(string id, List<string> newDeck) {
        if (players.ContainsKey(id)) {
            players[id].deck = new List<string>(newDeck);
        }
    }

    public void ClearAllPlayers() {
        players.Clear();
        allPlayers.Clear();
    }
}
