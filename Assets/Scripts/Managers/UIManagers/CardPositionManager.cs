using System;
using System.Collections.Generic;
using UnityEngine;

class CardPositionManager: MonoBehaviour {
    public static CardPositionManager Instance { get; private set; }
    public Canvas hand;
    private float hoveredGap = 10f;
    private float defaultGap = 120f;
    private float minX;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        RectTransform handRect = hand.GetComponent<RectTransform>();
        float midX = handRect.rect.center.x;
        minX = midX - defaultGap * 2f;
    }

    public void ResetCardPos(List<GameObject> cards) {
        for (int i=0; i<cards.Count; i++) {
            GameObject card = cards[i];
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            Vector2 position = new Vector2(minX + defaultGap * i, 64f);
            CardHoverEffect hoverEffect = cards[i].GetComponent<CardHoverEffect>();
            cards[i].GetComponent<CardAnimator>()?.MoveTo(position, () => hoverEffect.Init());
        }
    }

    public void RepositionCard(List<GameObject> cards, int hoveredIdx) {
        GameObject hoveredCard = cards[hoveredIdx];
        float hoveredCardX = hoveredCard.GetComponent<RectTransform>().anchoredPosition.x;
        for (int i=0; i<cards.Count; i++) {
            if (i == hoveredIdx) continue;
            GameObject card = cards[i];
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            CardHoverEffect hoverEffect = card.GetComponent<CardHoverEffect>();
            Vector2 position;
            if (i<hoveredIdx) {
                position = hoverEffect.originalPosition - new Vector3((cards.Count - (hoveredIdx - i + 1)) * hoveredGap, 0f, 0f);
            }
            else {
                position = hoverEffect.originalPosition + new Vector3((cards.Count - (i - hoveredIdx + 1)) * hoveredGap, 0f, 0f);
            }
            cards[i].GetComponent<CardAnimator>()?.MoveTo(position);
        }
    }
}
