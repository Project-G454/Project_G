using UnityEngine;

namespace WorldMap {
    public class MapNodeView: MonoBehaviour {
        public SpriteRenderer icon;

        public void SetView(Sprite iconSprite) {
            icon.sprite = iconSprite;
        }
    }
}
