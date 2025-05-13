
using Dices.Data;
using UnityEngine;

namespace Core.Loaders.Dices {
    public class DiceDataLoader {
        public static DiceD6Rotation LoadD6Rotation() {
            return Resources.Load<DiceD6Rotation>("Dices/D6");
        }
    }
}
