using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entities;
using Entities.Factories;
using Core.Interfaces;
using Entities.Handlers;
using Entities.Models;

namespace Core.Entities {
    /// <summary>
    /// 管理所有場上的 Entity（玩家與敵人）。
    /// </summary>
    public class EntityManager: MonoBehaviour, IManager {
        public GameObject dummy;
        public Transform entities;
        public static EntityManager Instance { get; private set; }
        private readonly Dictionary<int, Entity> entityDict = new();
        private readonly Dictionary<int, GameObject> entityObjectDict = new();
        private int nextEntityId = 100;

        public void Init() { }

        private void Awake() {
            Instance = this;
        }

        public void Reset() {}

        public Entity CreateEntity(EntityData entityData, Vector3 position) {
            GameObject newEntity = Instantiate(dummy, entities);
            Transform rectTransform = newEntity.GetComponent<Transform>();
            if (rectTransform != null) {
                rectTransform.position = position;
            }

            EntityBehaviour eb = newEntity.GetComponent<EntityBehaviour>();
            int id;
            if (entityData is PlayerData playerData) {
                id = playerData.playerId;
            } else {
                id = nextEntityId++;
            }
            Entity entity = EntityFactory.MakeEntity(id, entityData);
            entity.position = position;

            eb.Init(entity);

            SetupCharacterVisual(newEntity, entityData.entityClass);

            entity.avatar = CaptureHeadSetFromEntity(newEntity);
            var playerState = PlayerStateManager.Instance.GetPlayer(id);
            if (playerState != null) {
                playerState.avatar = entity.avatar;
            }

            RegisterEntity(entity, newEntity);
            InitDeckAndEnergy(entity);
            return entity;
        }

        private void SetupCharacterVisual(GameObject entityObject, EntityClasses entityClass) {
            // 嘗試獲取 CharacterVisualHandler 組件
            CharacterVisualHandler visualHandler = entityObject.GetComponentInChildren<CharacterVisualHandler>();

            if (visualHandler != null) {
                // 設置對應的外觀
                visualHandler.SetVisual(entityClass);
            }
            else {
                Debug.LogWarning($"無法找到 CharacterVisualHandler 組件！Entity: {entityObject.name}, Class: {entityClass}");
            }

            CharacterVisualSO visual = visualHandler.GetVisual(entityClass);
        }

        public void RegisterEntity(Entity entity, GameObject entityObject) {
            if (!entityDict.ContainsKey(entity.entityId)) {
                entityDict.Add(entity.entityId, entity);
                entityObjectDict.Add(entity.entityId, entityObject);
            }
        }

        public void UnregisterEntity(int entityId) {
            if (entityDict.ContainsKey(entityId)) {
                entityDict.Remove(entityId);

                if (entityObjectDict.TryGetValue(entityId, out var obj)) {
                    Destroy(obj);
                    entityObjectDict.Remove(entityId);
                }
            }
        }

        public Entity GetEntity(int entityId) {
            entityDict.TryGetValue(entityId, out var entity);
            return entity;
        }

        public GameObject GetEntityObject(int entityId) {
            entityObjectDict.TryGetValue(entityId, out var entity);
            return entity;
        }

        public List<Entity> GetEntityList() {
            return new List<Entity>(entityDict.Values);
        }

        public List<Entity> GetEntitiesByType(EntityTypes type) {
            return entityDict.Values.Where(e => e.type == type).ToList();
        }

        public List<GameObject> GetEntitiesObjectByType(EntityTypes type) {
            List<GameObject> result = new();

            foreach (var pair in entityDict) {
                int entityId = pair.Key;
                Entity entity = pair.Value;

                if (entity.type == type && entityObjectDict.TryGetValue(entityId, out var obj)) {
                    result.Add(obj);
                }
            }

            return result;
        }

        public void ClearAllEntities() {
            foreach (var entityId in entityDict.Keys.ToList()) {
                UnregisterEntity(entityId);
            }
        }

        private void InitDeckAndEnergy(Entity entity) {
            entity.deckManager.InitializeDeck();
            entity.energyManager.InitializeEnergy();
        }

        public Sprite CaptureHeadSetFromEntity(GameObject entityObject, int resolution = 128) {
            // 1. 找到 HeadSet
            Transform headSet = FindHeadSetFromEntity(entityObject);

            if (headSet == null) {
                Debug.LogWarning("找不到 HeadSet");
                return null;
            }

            // 2. 建立 RenderTexture
            RenderTexture rt = new RenderTexture(resolution, resolution, 24);
            Camera cam = new GameObject("TempHeadCam").AddComponent<Camera>();
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = new Color(0, 0, 0, 0); // 透明
            cam.targetTexture = rt;
            cam.cullingMask = LayerMask.GetMask("HeadCapture");

            // 3. 將 HeadSet 下所有子物件設成專用 Layer
            var headCaptureLayer = LayerMask.NameToLayer("HeadCapture");

            var allChildren = headSet.GetComponentsInChildren<Transform>(true);
            var originalLayers = new Dictionary<GameObject, int>();

            foreach (var child in allChildren) {
                originalLayers[child.gameObject] = child.gameObject.layer;
                child.gameObject.layer = headCaptureLayer;
            }

            // 4. 調整相機位置與尺寸（你可以依需要調整）
            cam.transform.position = headSet.position + new Vector3(0, 0, -10);
            cam.orthographicSize = 0.8f;

            // 5. 拍照
            cam.Render();
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            tex.Apply();

            Texture2D trimmed = TrimTransparent(tex);

            // 6. 還原 Layer
            foreach (var kvp in originalLayers) {
                kvp.Key.layer = kvp.Value;
            }

            // 7. 清除
            RenderTexture.active = null;
            cam.targetTexture = null;
            GameObject.Destroy(rt);
            GameObject.Destroy(cam.gameObject);

            // 8. 轉成 Sprite
            return Sprite.Create(trimmed, new Rect(0, 0, trimmed.width, trimmed.height), new Vector2(0.5f, 0.5f));
        }

        public static Texture2D TrimTransparent(Texture2D sourceTex)
        {
            int width = sourceTex.width;
            int height = sourceTex.height;
            Color32[] pixels = sourceTex.GetPixels32();

            int minX = width, maxX = 0, minY = height, maxY = 0;
            bool hasPixel = false;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Color32 c = pixels[y * width + x];
                    if (c.a > 0) {
                        hasPixel = true;
                        minX = Mathf.Min(minX, x);
                        maxX = Mathf.Max(maxX, x);
                        minY = Mathf.Min(minY, y);
                        maxY = Mathf.Max(maxY, y);
                    }
                }
            }

            if (!hasPixel)
                return sourceTex; // 全透明，直接回傳

            int croppedWidth = maxX - minX + 1;
            int croppedHeight = maxY - minY + 1;

            Texture2D trimmed = new Texture2D(croppedWidth, croppedHeight, TextureFormat.RGBA32, false);
            trimmed.SetPixels(sourceTex.GetPixels(minX, minY, croppedWidth, croppedHeight));
            trimmed.Apply();

            return trimmed;
        }

        public Transform FindHeadSetFromEntity(GameObject character) {
            // 先找出所有子物件中名為 HeadSet 的 Transform
            var allChildren = character.GetComponentsInChildren<Transform>(true);

            foreach (var child in allChildren) {
                if (child.name == "HeadSet")
                    return child;
            }

            Debug.LogWarning("找不到 HeadSet");
            return null;
        }
    }
}
