using Cards;
using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler: MonoBehaviour, IDropHandler {
    void IDropHandler.OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            GameObject card = eventData.pointerDrag;
            CardBehaviour cb = card.GetComponent<CardBehaviour>();
            CardManager cm = CardManager.Instance;
            if (cm.UseCard(cb.card, 999)) Destroy(card);
        }
    }
}
