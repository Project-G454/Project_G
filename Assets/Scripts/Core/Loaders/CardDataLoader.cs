using System.Collections.Generic;
using System.Linq;
using Cards.Data;
using UnityEngine;

namespace Core.Loaders.Cards {
    public class CardDataLoader {
        public static List<CardData> LoadAll() {
            return Resources.LoadAll<CardData>("Cards/Data").ToList();
        }

        public static List<Sprite> LoadAssets() {
            return Resources.LoadAll<Sprite>("Cards/CardAssets").ToList();
        }

        public static Sprite LoadBackground(int id) {
            return Resources.Load<Sprite>("Cards/Backgrounds/Card_Background_" + id.ToString());
        }
    }
}
