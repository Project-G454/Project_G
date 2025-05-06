using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.Animations;
using Cards.Data;
using Cards.Factories;
using Cards.Helpers;
using Core.Interfaces;
using Core.Loaders.Cards;
using Core.Managers.Deck;
using Entities;
using Entities.Categories;
using UnityEngine;

namespace Core.Managers.Cards {
    public class CardManager: MonoBehaviour, IManager {
        public static CardManager Instance { get; private set; }
        public GameObject cardPrefab;
        public Transform cardParent;
        private BattleManager _battleManager;
        private DeckManager _deckManager;
        private static Dictionary<int, CardData> _cardDict = new();
        public static readonly List<GameObject> cardList = new();
        public static List<Sprite> cardAssets = new();
        public bool isTurnFinished = true;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void Init() {
            _battleManager = BattleManager.Instance;
            _deckManager = (_battleManager.currentEntity as Player)?.deckManager;

            List<CardData> cardData = CardDataLoader.LoadAll();
            cardAssets = CardDataLoader.LoadAssets();
            cardData.ForEach(e => {
                if (!_cardDict.ContainsKey(e.id)) _cardDict.Add(e.id, e);
            });
        }

        public CardData GetCardById(int id) {
            return _cardDict.GetValueOrDefault(id);
        }

        public void Add(CardData cardData) {
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
            this.isTurnFinished = false;

            if (_battleManager.currentEntity is Player player) {
                _deckManager = player.deckManager;
                _deckManager.DrawCards(5);

                foreach (int id in _deckManager.hand.GetAllCards()) {
                    CardData cardData = GetCardById(id);
                    Add(cardData);
                }

                CardAnimation.Deal(cardParent, cardList);

                foreach (GameObject cardObj in cardList) {
                    CardView view = cardObj.GetComponent<CardView>();
                    view.RecordInitialState();
                }
            }
        }

        public void EndTurn() {
            isTurnFinished = true;

            _deckManager.DiscardHand();
            ResetCardObjects();
        }

        public void UseCard(CardBehaviour cb, int targetId) {
            if (isTurnFinished) return;

            Entity currentEntity = _battleManager.currentEntity;
            if (!_deckManager.hand.GetAllCards().Contains(cb.card.id)) {
                Debug.Log("Card not found!");
                return;
            }

            cb.card.Use(currentEntity.entityId, targetId);   // Apply card effect
            _deckManager.Use(cb.card.id);                    // Remove card from deck
        }

        public void SetNewCardPosition() {
            List<Vector3> cardsPos = CardPositionHelper.CalcCardPosition(cardParent, cardList);
            for (int i=0; i<cardList.Count(); i++) {
                CardView view = cardList[i].GetComponent<CardView>();
                view.SetInitialState(cardsPos[i], view.GetInitialScale(), view.GetInitialSiblingIdx());
            }
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
