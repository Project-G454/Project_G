using System.Collections.Generic;

namespace Entities.Models {
    public class ClassInfo {
        public List<int> Deck { get; set; }
        public int Hp { get; set; }

        public ClassInfo(List<int> deck, int hp) {
            Deck = deck;
            Hp = hp;
        }
    }
}
