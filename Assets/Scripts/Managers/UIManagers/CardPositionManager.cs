using System.Collections.Generic;
using Cards.Animations;
using Core.Interfaces;
using UnityEngine;

class CardPositionManager: MonoBehaviour, IManager {
    public static CardPositionManager Instance { get; private set; }
    public Canvas hand;
    private const float HOVERED_GAP = 10f;
    private const float DEFAULT_GAP = 240f;
    private float _minX;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init() {
        _minX = DEFAULT_GAP * -2f;
    }

    public void ResetCardPos(List<GameObject> cards) {
        for (int i = 0; i < cards.Count; i++) {
            GameObject card = cards[i];
            Vector2 position = new Vector2(_minX + DEFAULT_GAP * i, 128f);
            CardHoverEffect hoverEffect = cards[i].GetComponent<CardHoverEffect>();
            cards[i].GetComponent<CardAnimator>()?.MoveTo(position, () => hoverEffect.Init());
        }
    }

    public void RepositionCard(List<GameObject> cards, int hoveredIdx) {
        GameObject hoveredCard = cards[hoveredIdx];
        float hoveredCardX = hoveredCard.GetComponent<RectTransform>().anchoredPosition.x;
        for (int i = 0; i < cards.Count; i++) {
            if (i == hoveredIdx) continue;
            GameObject card = cards[i];
            CardHoverEffect hoverEffect = card.GetComponent<CardHoverEffect>();
            Vector2 position;
            if (i < hoveredIdx) {
                position = hoverEffect.originalPosition - new Vector3((cards.Count - (hoveredIdx - i + 1)) * HOVERED_GAP, 0f, 0f);
            }
            else {
                position = hoverEffect.originalPosition + new Vector3((cards.Count - (i - hoveredIdx + 1)) * HOVERED_GAP, 0f, 0f);
            }
            cards[i].GetComponent<CardAnimator>()?.MoveTo(position);
        }
    }
}
