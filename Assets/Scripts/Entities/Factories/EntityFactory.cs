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
            { EntityClasses.WARRIOR, new ClassInfo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, 100) },
            { EntityClasses.RANGER,  new ClassInfo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, 80)  },
            { EntityClasses.ROGUE,   new ClassInfo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, 70)  },
            { EntityClasses.WIZARD,  new ClassInfo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, 60)  }
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
