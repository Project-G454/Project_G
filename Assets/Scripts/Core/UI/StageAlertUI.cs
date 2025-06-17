using System;
using Core.Managers.WorldMap;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldMap;

namespace Core.UI {
    public class StageAlertUI: MonoBehaviour {
        public TMP_Text stageText;
        public TMP_Text nodeTypeText;
        public TMP_Text descriptionText;
        public Button confirmButton;
        public Button cancelButton;
        public Vector3 hidePosition = new Vector3(0, -800, 0);
        public Vector3 showPosition = new Vector3(0, 0, 0);
        private RectTransform _rt;

        public void Start() {
            _rt = GetComponent<RectTransform>();
            cancelButton.onClick.AddListener(() => {
                Hide();
            });
            Hide();
        }

        public void Show(MapNode node, Action OnComfirm = null, Action OnCancel = null) {
            int stage = node.stage;
            int level = WorldMapManager.Instance.level;
            string title = $"{level}-{stage}";
            stageText.text = title;
            nodeTypeText.text = node.data.nodeName;
            descriptionText.text = node.data.description;

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(() => {
                Hide(OnComfirm);
            });

            cancelButton.onClick.AddListener(() => {
                OnCancel?.Invoke();
                Hide();
            });

            gameObject.SetActive(true);

            _rt.DOAnchorPos(showPosition, 0.5f).SetEase(Ease.OutBack).OnComplete(() => {
                _rt.anchoredPosition = showPosition;
            });
        }

        public void Hide(Action OnComplete = null) {
            cancelButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
            _rt.DOAnchorPos(hidePosition, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
                gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
        }
    }
}
