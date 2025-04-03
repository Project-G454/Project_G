using System.Collections.Generic;
using Cards.Data;
using Core.Interfaces;
using UnityEngine;

namespace Core.Loaders.Cards {
    public class CardDataLoader: MonoBehaviour, IManager {
        public static CardDataLoader Instance { get; private set; }
        private Dictionary<int, CardData> cardLookUp;
        private CardData[] _allCards;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _allCards = Resources.LoadAll<CardData>("Cards/Data");
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            cardLookUp = new Dictionary<int, CardData>();

            foreach (var card in _allCards) {
                if (card != null && !cardLookUp.ContainsKey(card.id)) {
                    cardLookUp.Add(card.id, card);
                }
            }
        }

        public CardData GetCardById(int id) {
            if (cardLookUp == null) Init();
            return cardLookUp.TryGetValue(id, out var data)? data: null;
        }
    }
}
