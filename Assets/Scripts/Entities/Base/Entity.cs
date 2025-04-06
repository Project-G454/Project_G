using System;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Managers.Deck;
using Core.Managers.Energy;
using Effects;
using UnityEngine;

namespace Entities {
    /// <summary>
    /// 通用的遊戲實體類別，可為玩家或敵人。
    /// </summary>
    public class Entity {
        public int entityId;
        public int health;
        public int maxHealth;
        public string entityName;
        public EntityTypes type;
        public EntityClasses entityClass;
        public EntityData entityData;
        public List<Effect> effects = new();
        public DeckManager deckManager;
        public EnergyManager energyManager;

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

        public void TakeDamage(int amount) {
            health = Mathf.Max(health - amount, 0);
            Debug.Log(String.Format("{0}.{1} : {2}/{3}", entityId, entityName, health, maxHealth));
        }

        public void Heal(int amount) {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public bool IsDead() => health <= 0;

        public void AddEffect(Effect effect) {
            effects.Add(effect);
            if (effect is IEnergySource energySource) {
                energyManager.RegisterSource(energySource);
                energyManager.UpdateEnergyRecover();
            }
        }

        public void RemoveEffect(Effect effect) {
            effects.Remove(effect);
            if (effect is IEnergySource energySource) {
                energyManager.UnregisterSource(energySource);
                energyManager.UpdateEnergyRecover();
            }
        }

        public List<Effect> GetEffectList() {
            return effects;
        }
    }
}
