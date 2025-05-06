using Cards.Data;
using Core.Entities;
using Core.Managers.Cards;
using Entities;
using Systems.Interactions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    public class UseCardHandler: MonoBehaviour{
        private CardBehaviour _cardBehaviour;
        private CardManager _cardManager;
        void Init() {
            _cardBehaviour = GetComponent<CardBehaviour>();
            _cardManager = CardManager.Instance;
        }

        public void Start() {
            Init();
        }

        public void UseCard() {
            var targetId = GetTargetId();
            if (targetId == null) return;

            Entity target = EntityManager.Instance.GetEntity((int)targetId);
            _cardManager.UseCard(_cardBehaviour, target.entityId);
        }

        public int? GetTargetId() {
            // UI to World Raytrace
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                Receiver receiver = hit.collider.GetComponent<Receiver>();
                if (receiver == null || !receiver.HasReceiver(ReceiverType.Card)) return null;

                EntityBehaviour entityBehaviour = hit.collider.GetComponent<EntityBehaviour>();
                if (entityBehaviour == null) return null;

                int targetId = entityBehaviour.entity.entityId;
                return targetId;
            }
            return null;
        }
    }
}
