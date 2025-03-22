using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler: 
    MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerClickHandler, 
    IPointerDownHandler, 
    IPointerUpHandler,
    IDragHandler,
    IBeginDragHandler,
    IEndDragHandler {
    public GameObject card;
    private RectTransform reacTransform;
    private CanvasGroup cg;
    private bool isDragged = false;
    private void Awake() {
        reacTransform = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        Debug.Log("Clicked");
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        card.transform.localScale += new Vector3(0.05f, 0.05f, 0f);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        card.transform.localScale -= new Vector3(0.05f, 0.05f, 0f);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        if (!isDragged) card.transform.position += new Vector3(0f, 20f, 0f);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        if (!isDragged) card.transform.position -= new Vector3(0f, 20f, 0f);
        if (!eventData.dragging && isDragged) isDragged = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        reacTransform.anchoredPosition += eventData.delta;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        cg.blocksRaycasts = false;
        isDragged = true;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        cg.blocksRaycasts = true;
        int gridSize = 50;
        reacTransform.anchoredPosition = new Vector2(
            gridSize * (int)(reacTransform.anchoredPosition.x / gridSize),
            gridSize * (int)((reacTransform.anchoredPosition.y + reacTransform.rect.height) / gridSize) - gridSize * (int)(reacTransform.rect.height / gridSize)
        );
    }

}
