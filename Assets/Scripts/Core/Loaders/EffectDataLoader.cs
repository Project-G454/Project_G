using System.Collections.Generic;
using System.Linq;
using Effects.Data;
using UnityEngine;

namespace Core.Loaders.Effects {
    public class EffectDataLoader {
        public static List<EffectData> LoadAll() {
            return Resources.LoadAll<EffectData>("Effects").ToList();
        }
    }
}
