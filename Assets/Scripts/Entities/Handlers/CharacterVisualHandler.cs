using System.Collections.Generic;
using Entities.Categories;
using Entities.Models;
using UnityEngine;

namespace Entities.Handlers {
    public class CharacterVisualHandler : MonoBehaviour {
        [SerializeField] private SpriteRenderer characterRenderer;
        [SerializeField] private Animator characterAnimator;

        // 用於編輯器中設置，也可以動態加載
        [SerializeField] private CharacterVisualSO[] characterVisuals;
        
        private Dictionary<EntityClasses, CharacterVisualSO> _visualsMap;

        private void Awake() {
            InitializeVisualsMap();
        }

        private void InitializeVisualsMap() {
            _visualsMap = new Dictionary<EntityClasses, CharacterVisualSO>();
            
            foreach (var visual in characterVisuals) {
                if (visual != null) {
                    _visualsMap[visual.characterClass] = visual;
                }
            }
        }

        public void SetVisual(EntityClasses characterClass) {
            if (_visualsMap.TryGetValue(characterClass, out CharacterVisualSO visual)) {
                ApplyVisual(visual);
            } else {
                Debug.LogWarning($"No visual found for character class: {characterClass}");
            }
        }

        private void ApplyVisual(CharacterVisualSO visual) {
            if (characterRenderer != null) {
                characterRenderer.sprite = visual.characterSprite;
                characterRenderer.color = visual.characterColor;
            }

            if (characterAnimator != null && visual.animatorController != null) {
                characterAnimator.runtimeAnimatorController = visual.animatorController;
            }
        }
    }
}
