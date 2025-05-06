using System.Collections.Generic;
using UnityEngine;

namespace Systems.Interactions {
    public enum ReceiverType {
        Card
    }

    public class Receiver: MonoBehaviour {
        public List<ReceiverType> receivers;

        public bool HasReceiver(ReceiverType tag) {
            return receivers.Contains(tag);
        }
    }
}
