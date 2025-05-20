using System.Collections.Generic;
using Core.Entities;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI {
    public class TurnPanelUI: MonoBehaviour {
        [SerializeField] private GameObject _turnSlotPrefab;
        [SerializeField] private Transform _turnPanelContainer;
        public List<GameObject> turnslots;

        public void UpdateTurnOrder(List<int> orderIds, int currentIndex) {
            List<Entity> entities = new();
            EntityManager entityManager = EntityManager.Instance;

            foreach (int orderId in orderIds) {
                entities.Add(entityManager.GetEntity(orderId));
            }

            foreach (Transform child in _turnPanelContainer)
                Destroy(child.gameObject);

            turnslots.Clear();

            foreach (var entity in entities) {
                var slot = Instantiate(_turnSlotPrefab, _turnPanelContainer);
                slot.transform.Find("Avatar").GetComponent<Image>().sprite = entity.avatar;

                var turnSlotUI = slot.transform.GetComponent<TurnSlotUI>();
                turnSlotUI.Init(entity);

                int entityIndex = orderIds.IndexOf(entity.entityId);
                bool isCurrent = (entityIndex == currentIndex);
                turnSlotUI.AnimateFocus(isCurrent);
                turnSlotUI.SetOutline();
                turnSlotUI.UpdateGrayOverLay(entityIndex < currentIndex);
            }
        }
    }
}
