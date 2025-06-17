using System.Collections.Generic;
using Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI {
    public class PlayerStatusOverlay: MonoBehaviour {
        public GameObject statusPrefab;
        public RectTransform viewport;
        private List<GameObject> statusRows = new();

        public void Set() {
            DontDestroyOnLoad(gameObject);
            PlayerStateManager stateManager = PlayerStateManager.Instance;
            List<GamePlayerState> players = stateManager.GetAllPlayer();

            int objCount = statusRows.Count - 1;
            for (int i = 0; i < players.Count; i++) {
                GamePlayerState status = players[i];
                if (i > objCount) _CreateRow(status);
                else {
                    GameObject row = statusRows[i];
                    PlayerStatusView view = row.GetComponent<PlayerStatusView>();
                    view.Set(status);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(viewport);
        }

        private void _CreateRow(GamePlayerState status) {
            GameObject statusRow = Instantiate(statusPrefab, viewport);
            PlayerStatusView view = statusRow.GetComponent<PlayerStatusView>();
            view.Set(status);
            statusRows.Add(statusRow);
        }

        public void Hide() {
            if (gameObject == null) return;
            gameObject.SetActive(false);
        }

        public void Show() {
            if (gameObject == null) return;
            Set();
            gameObject.SetActive(true);
        }
    }
}
