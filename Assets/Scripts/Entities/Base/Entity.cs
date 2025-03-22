using System;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// 通用的遊戲實體類別，可為玩家或敵人。
    /// </summary>
    public class Entity
    {
        public int entityId;
        public int health;
        public int maxHealth;
        public string entityName;
        public EntityTypes type;
        public EntityClasses entityClass;
        public EntityData entityData;

        public Entity(
            int id,
            EntityData entityData
        ) {
            this.entityId = id;
            this.entityName = entityData.entityName;
            this.type = entityData.type;
            this.health = entityData.maxHealth;
            this.maxHealth = entityData.maxHealth;
            this.entityClass = entityData.entityClass;
            this.entityData = entityData;
        }

        public void TakeDamage(int amount)
        {
            health = Mathf.Max(health - amount, 0);
            Debug.Log(String.Format("{0}.{1} : {2}/{3}", entityId, entityName, health, maxHealth));
        }

        public void Heal(int amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public bool IsDead() => health <= 0;
    }
}
