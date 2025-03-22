using UnityEngine;
using UnityEngine.EventSystems;

namespace Entities {
    class EntityBehaviour: MonoBehaviour {
        public Entity entity;

        public void Init(Entity entity) {
            this.entity = entity;
        }

        void OnMouseDown() {
            entity.TakeDamage(10);
        }
    }
}
