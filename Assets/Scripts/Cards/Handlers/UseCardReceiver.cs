using UnityEngine;

namespace Cards.Handlers {
    public class UseCardReceiver: MonoBehaviour {
        public void OnDrop(GameObject card) {
            Debug.Log("Drop Card");
        }
    }
}
