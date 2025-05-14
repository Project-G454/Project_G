// Character Scriptable Object
using Entities.Categories;
using UnityEngine;

namespace Entities.Models {
    [CreateAssetMenu(fileName = "CharacterVisual", menuName = "Game/Character Visual")]
    public class CharacterVisualSO : ScriptableObject {
        public EntityClasses characterClass;
        public GameObject characterPrefab;
        public Color characterColor = Color.white;
        public RuntimeAnimatorController animatorController;
    }
}
