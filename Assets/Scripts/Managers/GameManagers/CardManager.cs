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
        private DeckManager _deckManager;
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
            if (_battleManager == null) this._battleManager = BattleManager.Instance;

            if (_battleManager.currentEntity is Player player) {
                this._deckManager = player.deckManager;
                this._deckManager.DrawCards(5);
                int i = 0;
                foreach (int id in this._deckManager.hand.GetAllCards()) {
                    CreateCard(CardFactory.GetFakeCardData(id), new Vector2(50 + 150*i++, 64));
                }
            }
        }

        public void EndTurn() {
            this._isCardState = false;
            
            this._deckManager.DiscardHand();
            ResetCardObjects();

            _battleManager.OnCardPlayed();
        }

        public bool UseCard(CardBehaviour cb, int targetId) {
            if (!this._isCardState) return false;

            Entity currentEntity = _battleManager.currentEntity;

            cb.card.Use(currentEntity.entityId, targetId);  // Apply card effect
            this._deckManager.Use(cb.card.id);              // Remove card from deck
            Destroy(cb.cardObject);                         // Destroy card GameObject
            cardList.Remove(cb.cardObject);                 // Remove card from GameObject list
            
            return this._isCardState;
        }

        public void ResetCardObjects() {
            foreach (GameObject cardObject in cardList) {
                Destroy(cardObject);
            }
            cardList.Clear();
        }

        // public void Remove(int cardId)
        // {
        //     cardList.Remove(this.GetCard(cardId));
        // }

        // public void Remove(Card card)
        // {
        //     cardList.Remove(card);
        // }

        // public Card GetCard(int cardId)
        // {
        //     return cardList.FirstOrDefault(card => card.id == cardId);
        // }
    }
}
