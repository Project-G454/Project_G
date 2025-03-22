using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler: MonoBehaviour, IDropHandler {
    void IDropHandler.OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            GameObject card = eventData.pointerDrag;
            CardBehaviour cb = card.GetComponent<CardBehaviour>();
            cb.card.Use(1, 2);
            Destroy(card);
        }
    }
}
