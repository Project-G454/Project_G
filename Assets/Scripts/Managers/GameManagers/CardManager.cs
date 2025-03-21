using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.Categories;
using UnityEngine;

namespace Core.Managers.Cards {
    public class CardManager: MonoBehaviour {
        public static CardManager Instance { get; private set; }
        public GameObject cardPrefab;
        public Transform cardParent;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() {
            CardData data1 = new CardData(
                1,
                0,
                "Card 1",
                "Some Description",
                CardTypes.ATTACK,
                new string[] { "All" },
                CardRarity.COMMON
            );
            CardData data2 = new CardData(
                2,
                1,
                "Card 2",
                "Some Description",
                CardTypes.MAGIC,
                new string[] { "All" },
                CardRarity.UNCOMMON
            );
            CardData data3 = new CardData(
                7,
                2,
                "Card 3",
                "Some Description",
                CardTypes.DEFENCE,
                new string[] { "All" },
                CardRarity.RARE
            );
            CreateCard(data1, new Vector3(-300, 10, 0));
            CreateCard(data2, new Vector3(0, 10, 0));
            CreateCard(data3, new Vector3(300, 10, 0));
        }

        public void CreateCard(CardData cardData, Vector3 position) {
            GameObject newCard = Instantiate(cardPrefab, cardParent);

            RectTransform rectTransform = newCard.GetComponent<RectTransform>();
            if (rectTransform != null) {
                rectTransform.SetParent(cardParent, false); // 設定為 UI 子物件，保持本地座標
                rectTransform.localPosition = position;
            }

            CardView cardView = newCard.GetComponent<CardView>();
            if (cardView != null) {
                Card card = new Card(cardData);
                switch (cardData.type) {
                    case CardTypes.ATTACK:
                        card = new AttackCard(cardData, 5);
                        break;
                    case CardTypes.MAGIC:
                        card = new MagicCard(cardData, 5);
                        break;
                    case CardTypes.MOVE:
                        card = new MoveCard(cardData, 5);
                        break;    
                }
                cardView.SetCardView(card);
            }
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
