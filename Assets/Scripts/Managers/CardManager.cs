using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public class CardManager {
        private static readonly List<Card> cardList = new();
        
        public void Add(Card card)
        {
            cardList.Add(card);
        }

        public void Remove(int cardId)
        {
            cardList.Remove(this.GetCard(cardId));
        }

        public void Remove(Card card)
        {
            cardList.Remove(card);
        }

        public Card GetCard(int cardId)
        {
            return cardList.FirstOrDefault(card => card.id == cardId);
        }
    }
}