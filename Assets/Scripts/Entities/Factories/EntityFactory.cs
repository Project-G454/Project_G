using System.Collections.Generic;
using Entities.Categories;

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

        private static readonly Dictionary<EntityClasses, List<int>> classDeck = new() {
            //{ EntityClasses.WARRIOR, new List<int> { 101, 101, 101, 102, 102, 103, 103, 104, 105, 105, 106, 106, 107, 108, 109 } },
            // { EntityClasses.WARRIOR, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
            // { EntityClasses.RANGER, new List<int> { 6, 7, 8, 9, 10 } },
            // { EntityClasses.ROGUE, new List<int> { 11, 12, 13, 14, 15 } },
            // { EntityClasses.WIZARD, new List<int> { 16, 17, 18, 19, 20 } }
            { EntityClasses.WARRIOR, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
            { EntityClasses.RANGER, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
            { EntityClasses.ROGUE, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
            { EntityClasses.WIZARD, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } }
        };

        public static List<int> GetClassDeck(EntityClasses entityClass)
        {
            return classDeck.ContainsKey(entityClass) ? new List<int>(classDeck[entityClass]) : new List<int>();
        }
    }
}
