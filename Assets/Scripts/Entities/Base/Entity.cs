using System;
using System.Collections.Generic;
using System.Linq;
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
        public event Action OnHpChanged;
        public event Action OnEffectsChanged;
        public event Action OnDeath;

        public int entityId;
        private int _currentHp;
        public int maxHp;
        public string entityName;
        public EntityTypes type;
        public EntityClasses entityClass;
        public EntityData entityData;
        public List<Effect> effects = new();
        public DeckManager deckManager;
        public EnergyManager energyManager;
        public Vector2 position;
        public Sprite avatar;

        public int currentHp {
            get => _currentHp;
            set {
                _currentHp = Mathf.Clamp(value, 0, maxHp);
                OnHpChanged?.Invoke(); // 通知 UI

                if (_currentHp <= 0) {
                    OnDeath?.Invoke();
                }
            }
        }

        public Entity(
            int id,
            EntityData entityData
        ) {
            this.entityId = id;
            this.entityName = entityData.entityName;
            this.type = entityData.type;
            this.maxHp = entityData.maxHp;
            this.currentHp = entityData.currentHp;
            this.entityClass = entityData.entityClass;
            this.entityData = entityData;
        }

        public void TakeDamage(int amount) {
            currentHp -= amount;
            Debug.Log(String.Format("{0}.{1} : {2}/{3}", entityId, entityName, currentHp, maxHp));
        }

        public bool IsStunned() {
            foreach (Effect effect in effects) {
                if (effect.effectType == Effects.Data.EffectType.STUN) return true;
            }
            return false;
        }

        public void Heal(int amount) {
            currentHp += amount;
        }

        public bool IsDead() => currentHp <= 0;

        public void AddEffect(Effect newEffect) {
            // effects.Add(newEffect);
            // if (newEffect is IEnergySource energySource) {
            //     energyManager.RegisterSource(energySource);
            //     energyManager.UpdateEnergyRecover();
            // }
            var existing = effects.FirstOrDefault(e => e.id == newEffect.id);
            if (existing != null) {
                // 已有此效果 → 疊加回合
                existing.rounds += newEffect.rounds;

                // 如果效果有 UI 也要更新
                OnEffectsChanged?.Invoke();
            }
            else {
                // 全新效果 → 正常加入
                effects.Add(newEffect);
                OnEffectsChanged?.Invoke();
            }

            if (newEffect is IEnergySource energySource) {
                energyManager.RegisterSource(energySource);
                energyManager.UpdateEnergyRecover();
            }
        }

        public void RemoveEffect(Effect effect) {
            effects.Remove(effect);
            OnEffectsChanged?.Invoke();

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
