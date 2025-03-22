using Cards;
using Cards.Factories;
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
            CreateCard(data1, new Vector2(50, 64));
            CreateCard(data2, new Vector2(200, 64));
            CreateCard(data3, new Vector2(350, 64));
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
