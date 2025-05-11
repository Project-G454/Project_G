using UnityEngine;

namespace Entities.Models {
    [System.Serializable]
    public class CharacterVisualData {
        public Sprite characterSprite;
        public Color characterColor = Color.white;
        public RuntimeAnimatorController animatorController;
    }
}
