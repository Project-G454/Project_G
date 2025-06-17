using System.Collections.Generic;
using Entities.Categories;
using Entities.Models;

namespace Entities.Factories
{
    public static class EntityFactory {
        public static Entity MakeEntity(int id, EntityData entityData) {
            Entity entity = entityData.type switch {
                EntityTypes.PLAYER => new Player(id, entityData),
                EntityTypes.ENEMY => new Enemy(id, entityData),
                _ => null
            };

            return entity;
        }

        private static readonly Dictionary<EntityClasses, ClassInfo> classInfo = new()
        {
            // Class, Card IDs, Initial HP
            { EntityClasses.WARRIOR, new ClassInfo(new List<int> { 1, 1, 1, 2, 2, 3, 4, 4, 4, 5 }, 100) },
            { EntityClasses.RANGER,  new ClassInfo(new List<int> { 4, 6, 6, 6, 6, 7, 7, 8, 8, 9 }, 80)  },
            { EntityClasses.ROGUE,   new ClassInfo(new List<int> { 1, 10, 10, 10, 10, 11, 11, 12, 13, 13 }, 70)  },
            { EntityClasses.WIZARD,  new ClassInfo(new List<int> { 14, 14, 15, 15, 16, 17 }, 60)  },
            { EntityClasses.Boss,  new ClassInfo(new List<int> { 21 }, 300)  },
            { EntityClasses.Minion,  new ClassInfo(new List<int> { 1 }, 20)  }
        };


        public static List<int> GetClassDeck(EntityClasses entityClass)
        {
            return classInfo.ContainsKey(entityClass) ? new List<int>(classInfo[entityClass].Deck) : new List<int>();
        }

        public static int GetHp(EntityClasses entityClass)
        {
            return classInfo.ContainsKey(entityClass) ? classInfo[entityClass].Hp : 0;
        }
    }
}
