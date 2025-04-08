using System.Collections.Generic;
using System.Linq;
using Cards.Data;
using UnityEngine;

namespace Core.Loaders.Cards {
    public class CardDataLoader {
        public static List<CardData> LoadAll() {
            return Resources.LoadAll<CardData>("Cards/Data").ToList();
        }
    }
}
