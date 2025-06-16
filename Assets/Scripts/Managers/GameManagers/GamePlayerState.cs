using System.Collections.Generic;
using Entities;
using Entities.Factories;
using UnityEngine;

namespace Core.Game {
    [System.Serializable]
    public class GamePlayerState {
        public int playerId;
        public string playerName;
        public Sprite avatar;
        public EntityClasses entityClass;
        public List<int> deck = new List<int>();
        public int maxHp = 100;
        public int currentHp = 100;
        public int gold = 0;

        public GamePlayerState(int id, string name, EntityClasses entityClass, int gold) {
            this.playerId = id;
            this.playerName = name;
            this.entityClass = entityClass;
            this.deck = EntityFactory.GetClassDeck(entityClass);
            this.maxHp = EntityFactory.GetHp(entityClass);
            this.currentHp = this.maxHp;
            this.gold = gold;
        }
    }
}
