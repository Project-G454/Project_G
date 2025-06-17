using System.Collections.Generic;
using UnityEngine;
using Core.Game;
using Entities;
using Core.Interfaces;
using Entities.Handlers;
using Entities.Models;

public class PlayerStateManager : MonoBehaviour, IManager {
    public static PlayerStateManager Instance { get; private set; }
    [SerializeField] private List<GamePlayerState> allPlayers = new List<GamePlayerState>();
    private Dictionary<int, GamePlayerState> players = new Dictionary<int, GamePlayerState>();
    public Camera renderCam;
    

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(int id, string name, EntityClasses entityClass, int gold = 0) {
        CharacterVisualHandler visualHandler = GetComponent<CharacterVisualHandler>();
        if (!players.ContainsKey(id)) {
            CharacterVisualSO visualSO = visualHandler.GetVisual(entityClass);
            GameObject character = visualSO.characterPrefab;
            SPUM_Prefabs characterScript = character.GetComponent<SPUM_Prefabs>();
            Sprite headSprite = PrefabRenderer.RenderHeadToSprite(characterScript.head, renderCam, 1024, 1024);
            var state = new GamePlayerState(id, name, entityClass, gold, headSprite);
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
            players[id].currentHp += delta;
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

    public void Init() { }

    public void Reset() { }
}
