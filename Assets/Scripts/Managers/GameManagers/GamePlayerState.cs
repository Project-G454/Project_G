using System.Collections.Generic;

namespace Core.Game {
    [System.Serializable]
    public class GamePlayerState {
        public string playerId;
        public List<string> deck = new List<string>();
        public int hp = 100;
        public int gold = 0;

        public GamePlayerState(string id, List<string> deck, int hp, int gold) {
            this.playerId = id;
            this.deck = new List<string>(deck);
            this.hp = hp;
            this.gold = gold;
        }
    }
}
