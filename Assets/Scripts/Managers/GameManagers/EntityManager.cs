using System.Collections.Generic;
using UnityEngine;
using Entities;
using Entities.Categories;

namespace Core.Entities {
    /// <summary>
    /// 管理所有場上的 Entity（玩家與敵人）。
    /// </summary>
    public class EntityManager: MonoBehaviour {
        public static EntityManager Instance { get; private set; }

        private readonly Dictionary<int, Entity> entityDict = new();
        private int nextEntityId = 1;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public Entity CreateEntity(EntityData entityData, Vector3 position) {
            int id = nextEntityId++;
            Entity entity = entityData.type switch {
                EntityTypes.PLAYER => new Player(id, entityData),
                EntityTypes.ENEMY => new Enemy(id, entityData),
                _ => null
            };

            RegisterEntity(entity);
            return entity;
        }

        public void RegisterEntity(Entity entity) {
            if (!entityDict.ContainsKey(entity.entityId)) {
                entityDict.Add(entity.entityId, entity);
            }
        }

        public void UnregisterEntity(int entityId) {
            if (entityDict.ContainsKey(entityId)) {
                entityDict.Remove(entityId);
            }
        }

        public Entity GetEntity(int entityId) {
            entityDict.TryGetValue(entityId, out var entity);
            return entity;
        }
    }
}
