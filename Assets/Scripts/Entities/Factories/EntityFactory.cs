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
    }
}
