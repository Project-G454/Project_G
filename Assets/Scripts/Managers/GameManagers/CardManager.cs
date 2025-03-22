using Cards;
using Cards.Factories;
using Entities;
using UnityEngine;

namespace Core.Managers.Cards {
    public class CardManager: MonoBehaviour {
        public static CardManager Instance { get; private set; }
        public GameObject cardPrefab;
        public Transform cardParent;
        private BattleManager _battleManager;
        private bool _isCardState = false;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() {
            _battleManager = BattleManager.Instance;
            for (int i=0; i<15; i++) {
                CreateCard(CardFactory.GetFakeCardData(i), new Vector2(50 + 50*i, 64));
            }
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
                cb.Init(card);
            }
        }

        public void StartTurn() {
            this._isCardState = true;
        }

        public void EndTurn() {
            this._isCardState = false;
            _battleManager.OnCardPlayed();
        }

        public bool UseCard(Card card, int targetId) {
            Entity currentEntity = _battleManager.currentEntity;
            card.Use(currentEntity.entityId, targetId);
            return this._isCardState;
        }

        // private static readonly List<Card> cardList = new();

        // public void Add(Card card)
        // {
        //     cardList.Add(card);
        // }

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
