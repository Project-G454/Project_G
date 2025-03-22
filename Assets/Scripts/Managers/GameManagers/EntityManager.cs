using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entities;
using Entities.Factories;

namespace Core.Entities {
    /// <summary>
    /// 管理所有場上的 Entity（玩家與敵人）。
    /// </summary>
    public class EntityManager: MonoBehaviour {
        public GameObject dummy;
        public Transform entities;
        public static EntityManager Instance { get; private set; }
        private readonly Dictionary<int, Entity> entityDict = new();
        private readonly Dictionary<int, GameObject> entityObjectDict = new();
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
                EntityTypes.PLAYER,
                EntityClasses.WARRIOR
            );

            EntityData data2 = new EntityData(
                90,
                "Player2",
                EntityTypes.PLAYER,
                EntityClasses.RANGER
            );

            EntityData data3 = new EntityData(
                100,
                "Player3",
                EntityTypes.PLAYER,
                EntityClasses.WIZARD
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
            RegisterEntity(entity, newEntity);
            return entity;
        }

        public void RegisterEntity(Entity entity, GameObject entityObject) {
            if (!entityDict.ContainsKey(entity.entityId)) {
                entityDict.Add(entity.entityId, entity);
                entityObjectDict.Add(entity.entityId, entityObject);
            }
        }

        public void UnregisterEntity(int entityId) {
            if (entityDict.ContainsKey(entityId)) {
                entityDict.Remove(entityId);

                if (entityObjectDict.TryGetValue(entityId, out var obj)) {
                    Destroy(obj);
                    entityObjectDict.Remove(entityId);
                }
            }
        }

        public Entity GetEntity(int entityId) {
            entityDict.TryGetValue(entityId, out var entity);
            return entity;
        }

        public List<Entity> GetEntityList() {
            return new List<Entity>(entityDict.Values);
        }

        public List<Entity> GetEntitiesByType(EntityTypes type) {
            return entityDict.Values.Where(e => e.type == type).ToList();
        }

        public void ClearAllEntities() {
            foreach (var entityId in entityDict.Keys.ToList()) {
                UnregisterEntity(entityId);
            }
        }
    }
}
