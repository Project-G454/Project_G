using System.Collections.Generic;
using Core.Helpers;
using Core.Interfaces;
using Core.Loaders.Descriptions;
using Descriptions;
using Descriptions.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers {
    public class DescriptionManager: MonoBehaviour, IManager {
        public static DescriptionManager Instance { get; private set; }
        public GameObject descPrefab;
        public Transform descParent;
        private readonly Dictionary<int, DescriptionBehaviour> _descriptions = new();
        private bool _is_initialized = false; // 避免初始化兩次的保護

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init() {
            if (_is_initialized) return;

            List<DescriptionData> _descriptionData = DescriptionLoader.LoadAll();
            foreach (DescriptionData data in _descriptionData) {
                this.Add(data);
            }
            this.HideAll();
            _is_initialized = true;
        }

        public void Add(DescriptionData data) {
            if (_descriptions.ContainsKey(data.id)) {
                Debug.LogWarning($"Duplicate description ID: {data.id}");
                return;
            }

            GameObject obj = Instantiate(descPrefab, descParent);
            DescriptionBehaviour db = obj.GetComponent<DescriptionBehaviour>();
            if (db == null) {
                Debug.LogError("Description prefab is missing DescriptionBehaviour component.");
                return;
            }

            Description description = new Description(data);
            db.Init(obj, description);
            _descriptions.Add(data.id, db);
        }

        public DescriptionBehaviour GetById(int id) {
            _descriptions.TryGetValue(id, out var db);
            return db;
        }

        public void ShowDescriptions(int[] ids, RectTransform cardRT) {
            RectTransform lastRT = null;

            // 往右偏移一個卡片寬度
            float rightOffset = cardRT.rect.width * cardRT.lossyScale.x;

            foreach (int id in ids) {
                DescriptionBehaviour db = GetById(id);
                // RectTransform rt = db.view.GetComponent<RectTransform>();

                // // rt.SetParent(cardRT.parent, false);
                // rt.localScale = new Vector3(1.5f, 1.5f, 1f);

                // // 計算 base 世界 pivot 位置（以卡片或上一張為基準）
                // Vector3 baseWorldPos = (lastRT == null)
                //     ? UIPositionHelper.GetPivotWorldPosition(cardRT)
                //     : UIPositionHelper.GetPivotWorldPosition(lastRT);

                // // Y 軸往上 offset 疊加
                // float offsetY = (lastRT == null) ? 0f : (lastRT.rect.height * lastRT.lossyScale.y + 10f);
                // Vector3 worldUp = cardRT.root.TransformDirection(Vector3.up);
                // Vector3 worldRight = cardRT.root.TransformDirection(Vector3.right);

                // // ⭐️ 重點：加入往右偏移一個卡片寬度
                // Vector3 targetWorldPos = baseWorldPos + (worldUp * offsetY) + (worldRight * rightOffset);

                // lastRT = rt;

                // rt.position = targetWorldPos;

                db.view.Show();
            }
        }



        public void HideAll() {
            foreach (var db in _descriptions) {
                db.Value.view.Hide();
            }
        }
    }
}
