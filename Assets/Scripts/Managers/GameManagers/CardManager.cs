using System.Collections.Generic;
using Cards;
using Cards.Factories;
using Core.Managers.Deck;
using Entities;
using Entities.Categories;
using UnityEngine;

namespace Core.Managers.Cards {
    public class CardManager: MonoBehaviour {
        public static CardManager Instance { get; private set; }
        public GameObject cardPrefab;
        public Transform cardParent;
        private BattleManager _battleManager;
        private BattleManager BattleManager => _battleManager ??= BattleManager.Instance;
        private DeckManager _deckManager;
        private DeckManager DeckManager => _deckManager ??= (_battleManager.currentEntity as Player)?.deckManager;
        private bool _isCardState = false;

        private static readonly List<GameObject> cardList = new();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() {

        }

        public void CreateCard(CardData cardData, Vector2 position) {
            GameObject newCard = Instantiate(cardPrefab, cardParent);

            RectTransform rectTransform = newCard.GetComponent<RectTransform>();
            if (rectTransform != null) {
                rectTransform.SetParent(cardParent, false); // 設定為 UI 子物件，保持本地座標
                rectTransform.anchoredPosition = position;
            }

            CardView cardView = newCard.GetComponent<CardView>();
            CardBehaviour cb = newCard.GetComponent<CardBehaviour>();
            if (cardView != null) {
                Card card = CardFactory.MakeCard(cardData);
                cb.Init(newCard, card);
                cardList.Add(newCard);
            }
        }

        public void StartTurn() {
            this._isCardState = true;

            if (BattleManager.currentEntity is Player player) {
                _deckManager = player.deckManager;
                _deckManager.DrawCards(5);
                int i = 0;
                foreach (int id in _deckManager.hand.GetAllCards()) {
                    CardData fakeData = CardFactory.GetFakeCardData(id);
                    CreateCard(fakeData, new Vector2(50 + 150 * i++, 64));
                }
            }
        }

        public void EndTurn() {
            this._isCardState = false;

            DeckManager.DiscardHand();
            ResetCardObjects();

            BattleManager.OnCardPlayed();
        }

        public void UseCard(CardBehaviour cb, int targetId) {
            if (!this._isCardState) return;

            Entity currentEntity = BattleManager.currentEntity;
            if (!DeckManager.hand.GetAllCards().Contains(cb.card.id)) return;

            cb.card.Use(currentEntity.entityId, targetId);  // Apply card effect
            DeckManager.Use(cb.card.id);                    // Remove card from deck
            cb.DestroySelf();                               // Destroy card GameObject and Remove card from GameObject list
        }

        public void ResetCardObjects() {
            foreach (GameObject cardObject in cardList) {
                Destroy(cardObject);
            }
            cardList.Clear();
        }

        public void RemoveCard(GameObject cardObject) {
            cardList.Remove(cardObject);
        }
    }
}
