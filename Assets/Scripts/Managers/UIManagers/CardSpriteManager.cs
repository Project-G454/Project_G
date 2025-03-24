using System.Collections;
using System.IO;
using UnityEngine;

namespace Core.Managers.UI {
    public class CardSpriteManager: MonoBehaviour {
        public static CardSpriteManager Instance { get; private set; }
        public Sprite[] cardSprites;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            cardSprites = Resources.LoadAll<Sprite>("Cards/CardAssets");
            DontDestroyOnLoad(gameObject);
        }
    }
}
