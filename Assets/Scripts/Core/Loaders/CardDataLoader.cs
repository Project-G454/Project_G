using System.Collections.Generic;
using System.Linq;
using Cards.Data;
using Entities;
using UnityEngine;

namespace Core.Loaders.Cards {
    public class CardDataLoader {
        public static List<CardData> LoadAll() {
            return Resources.LoadAll<CardData>("Cards/Data").ToList();
        }

        public static List<CardData> LoadByClass(EntityClasses targetClass) {
            return Resources.LoadAll<CardData>("Cards/Data")
            .Where(card => card.classes.Contains(targetClass))
            .ToList();
        }

        public static List<Sprite> LoadAssets() {
            return Resources.LoadAll<Sprite>("Cards/CardAssets").ToList();
        }

        public static Sprite LoadBackground(int id) {
            return Resources.Load<Sprite>("Cards/Backgrounds/Card_Background_" + id.ToString());
        }

        public static Sprite LoadFrame(CardTypes type) {
            string color = type switch {
                CardTypes.ATTACK => "Red",
                CardTypes.MAGIC => "Purple",
                CardTypes.ENERGY => "Blue",
                CardTypes.HEAL => "Green",
                CardTypes.SUMMON => "Yellow",
                CardTypes.FUNCTIONAL => "Yellow",
                _ => "Red" // 預設值
            };
            return Resources.LoadAll<Sprite>("Cards/Card_v2").FirstOrDefault(e => e.name == $"Card_Type_{color}");
        }

        public static Sprite LoadTitleBG(CardRarity rarity) {
            string color = rarity switch {
                CardRarity.COMMON => "Gray",
                CardRarity.UNCOMMON => "Green",
                CardRarity.RARE => "Blue",
                CardRarity.EPIC => "Purple",
                CardRarity.LEGENDARY => "Orange",
                _ => "Gray" // 預設值
            };
            return Resources.LoadAll<Sprite>("Cards/Card_v2").FirstOrDefault(e => e.name == $"Card_Title_{color}");
        }

        public static Sprite LoadAttackIcon() {
            return LoadAssets().FirstOrDefault(e => e.name == $"Card_Type_ATTACK");
        }

        public static Sprite LoadHealIcon() {
            return LoadAssets().FirstOrDefault(e => e.name == $"Card_Type_Heal");
        }

        public static Sprite LoadEnergyIcon() {
            return LoadAssets().FirstOrDefault(e => e.name == $"CardAssets_17");
        }
    }
}
