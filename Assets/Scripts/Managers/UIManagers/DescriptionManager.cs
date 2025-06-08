using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards.Handlers;
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
        private List<int> _showing = new();
        private bool _is_initialized = false; // 避免初始化兩次的保護
        private Coroutine _clearCoroutine;

        private void Awake() {
            Instance = this;
        }

        public void Reset() {}

        public void Init() {
            if (_is_initialized) return;

            List<DescriptionData> _descriptionData = DescriptionLoader.LoadAll();
            foreach (DescriptionData data in _descriptionData) {
                this.Add(data);
            }
            Debug.Log($"Loaded {_descriptions.Count} descriptions.");
            this.HideAll();
            _is_initialized = true;
        }

        public void Start() {
            Init();
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

        public void ShowDescriptions(int[] ids) {
            foreach (int id in ids) {
                DescriptionBehaviour db = GetById(id);
                db.view.Show();
                _showing.Add(id);
            }
        }

        public void ShowOnly(int[] ids) {
            foreach (int id in ids) {
                if (!_showing.Contains(id)) {
                    DescriptionBehaviour db = GetById(id);
                    db.view.Show();
                    _showing.Add(id);
                }
            }

            List<int> readyToRemove = new();
            foreach (int id in _showing) {
                if (!ids.Contains(id)) {
                    DescriptionBehaviour db = GetById(id);
                    db.view.Hide();
                    readyToRemove.Add(id);
                }
            }

            foreach (int id in readyToRemove) {
                _showing.Remove(id);
            }
        }

        public void HideAll() {
            foreach (var db in _descriptions) {
                db.Value.view.Hide();
            }
            _showing.Clear();
        }
    }
}
