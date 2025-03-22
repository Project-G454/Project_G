using System.Collections.Generic;
using UnityEngine;
using Entities;
using Entities.Factories;
using UnityEditorInternal;

namespace Core.Entities {
    /// <summary>
    /// 管理所有場上的 Entity（玩家與敵人）。
    /// </summary>
    public class EntityManager: MonoBehaviour {
        public GameObject dummy;
        public Transform entities;
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

        void Start() {
            EntityData data1 = new EntityData(
                80,
                "Player1",
                EntityTypes.PLAYER
            );

            EntityData data2 = new EntityData(
                90,
                "Player2",
                EntityTypes.PLAYER
            );

            EntityData data3 = new EntityData(
                100,
                "Enemy1",
                EntityTypes.ENEMY
            );

            CreateEntity(data1, new Vector3(-5, 0, 0));
            CreateEntity(data2, new Vector3(0, 0, 0));
            CreateEntity(data3, new Vector3(5, 0, 0));
        }

        public Entity CreateEntity(EntityData entityData, Vector3 position) {
            GameObject newEntity = Instantiate(dummy, entities);
            Transform rectTransform = newEntity.GetComponent<Transform>();
            if (rectTransform != null) {
                rectTransform.position = position;
            }

            EntityBehaviour eb = newEntity.GetComponent<EntityBehaviour>();
            int id = nextEntityId++;
            Entity entity = EntityFactory.MakeEntity(id, entityData);

            eb.Init(entity);
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
