using UnityEngine;

namespace WorldMap {
    public class MapNodeView: MonoBehaviour {
        public SpriteRenderer icon;
        public Color lockedColor;
        public Color unlockColor;
        public Color resolvedColor;

        public void SetView(Sprite iconSprite) {
            icon.sprite = iconSprite;
        }

        public void Lock() {
            this.icon.color = lockedColor;
        }

        public void Unlock() {
            this.icon.color = unlockColor;
        }

        public void Resolve() {
            this.icon.color = resolvedColor;
        }
    }
}
