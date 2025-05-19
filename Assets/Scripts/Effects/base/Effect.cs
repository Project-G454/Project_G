using System;
using System.Collections.Generic;
using Core.Entities;
using Effects.Data;
using Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Effects {
    [Serializable]
    public class Effect {
        [SerializeField] public EffectData effectData;
        [SerializeField] public int rounds;
        public int id { get => effectData.id; }
        public string name { get => effectData.name; }
        public string effectDesc { get => effectData.effectDesc; }
        public Sprite icon { get; set; }
        public EffectType effectType { get; set; }
        public int behaviourId { get; set; }

        public Effect(EffectData effectData, int behaviourId, int rounds) {
            this.effectData = effectData;
            this.icon = effectData.icon;
            this.effectType = effectData.effectType;
            this.rounds = rounds;
            this.behaviourId = behaviourId;
        }

        public virtual void Init() {
            this.icon = effectData.icon;
            this.effectType = effectData.effectType;
        }

        public virtual Effect Copy() {
            Debug.Log("Copy");
            return new Effect(effectData, behaviourId, rounds);
        }

        public virtual void Trigger() {
            Debug.Log($"Effect_{id} -> Entity_{behaviourId}");
            rounds--;
        }
    }
}
