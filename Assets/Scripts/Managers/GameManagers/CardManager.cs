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
        private CardPositionManager _cardPositionManager;
        private bool _isCardState = false;
        public static readonly List<GameObject> cardList = new();

        private void Init() {
            _battleManager = BattleManager.Instance;
            _deckManager = (_battleManager.currentEntity as Player)?.deckManager;
            _cardPositionManager = CardPositionManager.Instance;
        }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() {
            Init();
        }

        public void CreateCard(CardData cardData) {
            GameObject newCard = Instantiate(cardPrefab, cardParent);

            RectTransform rectTransform = newCard.GetComponent<RectTransform>();
            if (rectTransform != null) {
                rectTransform.SetParent(cardParent, false); // 設定為 UI 子物件，保持本地座標
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

            if (_battleManager.currentEntity is Player player) {
                _deckManager = player.deckManager;
                _deckManager.DrawCards(5);

                foreach (int id in _deckManager.hand.GetAllCards()) {
                    CardData fakeData = CardFactory.GetFakeCardData(id);
                    CreateCard(fakeData);
                }

                _cardPositionManager.ResetCardPos(cardList);
            }
        }

        public void EndTurn() {
            this._isCardState = false;

            _deckManager.DiscardHand();
            ResetCardObjects();

            _battleManager.OnCardPlayed();
        }

        public void UseCard(CardBehaviour cb, int targetId) {
            if (!this._isCardState) return;

            Entity currentEntity = _battleManager.currentEntity;
            if (!_deckManager.hand.GetAllCards().Contains(cb.card.id)) return;

            cb.card.Use(currentEntity.entityId, targetId);   // Apply card effect
            _deckManager.Use(cb.card.id);                    // Remove card from deck
            cb.DestroySelf();                                // Destroy card GameObject and Remove card from GameObject list
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
