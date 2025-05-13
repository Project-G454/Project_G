using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Systems.Turn {
    public class NextRound: MonoBehaviour, IPointerClickHandler {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            CardManager cardManager = CardManager.Instance;
            cardManager.EndTurn();
        }
    }
}
