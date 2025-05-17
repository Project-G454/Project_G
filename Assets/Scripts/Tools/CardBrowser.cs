using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Cards.Data;
using Core.Loaders.Cards;
using Cards;

public class CardBrowser : EditorWindow {
    private CardTypes filterType = CardTypes.UNSET;
    private CardTypes prevFilterType = CardTypes.UNSET;
    private List<CardData> filteredCards = new();
    private enum SortType {
        None,
        ByID,
        ByName
    }

    private SortType sortType = SortType.None;
    private SortType prevSortType = SortType.None;
    private Vector2 scrollPos;

    [MenuItem("Tools/Card Browser")]
    public static void ShowWindow() {
        var window = GetWindow<CardBrowser>("Card Browser");
        Texture icon = EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image;
        window.titleContent = new GUIContent("Card Browser", icon);
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("卡牌分類過濾器", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        filterType = (CardTypes)EditorGUILayout.EnumPopup("卡牌類型", filterType);
        sortType = (SortType)EditorGUILayout.EnumPopup("排序方式", sortType);

        EditorGUILayout.Space();
        if (GUILayout.Button("重新整理") || prevFilterType != filterType || prevSortType != sortType) {
            LoadFilteredCards();
            prevFilterType = filterType;
            prevSortType = sortType;
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("ID", GUILayout.Width(40));
        EditorGUILayout.LabelField("Name");
        EditorGUILayout.LabelField("Type", GUILayout.Width(100));
        EditorGUILayout.LabelField("", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var card in filteredCards) {
            if (card == null) continue;

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(card.id.ToString(), GUILayout.Width(40));
            EditorGUILayout.LabelField(card.cardName);
            EditorGUILayout.LabelField(card.type.ToString(), GUILayout.Width(100));
            if (GUILayout.Button("選取", GUILayout.Width(50))) {
                Selection.activeObject = card;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void LoadFilteredCards() {
        List<CardData> cards = CardDataLoader.LoadAll();
        filteredCards.Clear();

        foreach (CardData card in cards) {
            if (card.type == filterType || filterType == CardTypes.UNSET) {
                filteredCards.Add(card);
            }
        }

        switch (sortType) {
            case SortType.ByID:
                filteredCards = filteredCards.OrderBy(e => e.id).ToList();
                break;
            case SortType.ByName:
                filteredCards = filteredCards.OrderBy(c => c.cardName).ToList();
                break;
            default:
                break;
        }
    }
}
