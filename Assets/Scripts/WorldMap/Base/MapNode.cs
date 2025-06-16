using System;
using System.Collections.Generic;
using System.Data.Common;
using Core.Helpers;
using Core.Managers;
using Core.Managers.WorldMap;
using Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using WorldMap.Animations;
using WorldMap.Models;
using Entities;
using Entities.Factories;

namespace WorldMap {
    public class MapNode: MonoBehaviour {
        public MapNodeData data;
        public int id;
        public Vector2 position;
        public int stage;
        public List<MapNode> connectedNodes = new();
        public List<MapNode> prevNodes = new();
        public MapNodeView view;
        public MapNodeAnimator animator;
        public bool isLocked = true;
        public bool isVisited = false;
        public bool isTail = false;

        public MapNode(MapNodeData data, int id, Vector2 pos) {
            this.data = data;
            this.id = id;
            this.position = pos;
        }

        public void Init(int id, MapNodeData data, LimitedNode node) {
            this.view = GetComponent<MapNodeView>();
            this.animator = GetComponent<MapNodeAnimator>();

            this.data = data;
            this.id = id;
            this.position = node.position;
            this.isLocked = node.type != NodeType.Start;
            this.isVisited = node.type == NodeType.Start;
            this.isTail = node.isTail;
            this.stage = node.position.y;

            this.view.Lock();
            if (data != null) view.SetView(data.icon);
        }

        public void Init(int id, MapNodeData data, NodeType type, Vector2 position) {
            this.view = GetComponent<MapNodeView>();
            this.animator = GetComponent<MapNodeAnimator>();

            this.data = data;
            this.id = id;
            this.isLocked = type != NodeType.Start;
            this.isVisited = type == NodeType.Start;
            this.position = position;

            this.view.Lock();
            if (data != null) view.SetView(data.icon);
        }

        public void Connect(MapNode node) {
            this.connectedNodes.Add(node);
            node.prevNodes.Add(this);
        }

        public void Disconnect(MapNode node) {
            this.connectedNodes.Remove(node);
            node.prevNodes.Remove(this);
        }

        public void OnMouseUp() {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
                
            if (this.isLocked || this.isVisited) {
                if (this.animator) {
                    animator.Shake();
                    animator.Blink(
                        isLocked ? view.lockedColor : view.resolvedColor,
                        1f
                    );
                }
                return;
            }

            WorldMapManager.Instance.currentNodeId = this.id;
            WorldMapManager.Instance.currentStage = this.stage;
            WorldMapManager.Instance.SaveCameraState();

            switch (this.data.nodeType) {
                case NodeType.Boss:
                    LoadSceneManager.Instance.LoadBattleScene(this, SetBossEntities());
                    break;
                case NodeType.Battle:
                    LoadSceneManager.Instance.LoadBattleScene(this, SetEntities());
                    break;
                case NodeType.Shop:
                    LoadSceneManager.Instance.LoadShopScene(this);
                    break;
                case NodeType.Recover:
                    LoadSceneManager.Instance.LoadRecoverScene(this);
                    break;
                default:
                    this.Resolve();
                    break;
            }
        }

        private List<EntityData> SetEntities() {
            List<EntityData> entityDatas = new();
            int count = 1;
            for (int i = 0; i < Math.Max(2, stage % 3); i++) {
                EntityClasses eclass = (EntityClasses) UnityEngine.Random.Range(1, 5);
                var hp = (int) (EntityFactory.GetHp(eclass) * 0.5);
                entityDatas.Add(new EntityData(
                    hp,
                    hp,
                    $"Enemy{count++}",
                    EntityTypes.ENEMY,
                    eclass
                ));
            }
            return entityDatas;
        }

        private List<EntityData> SetBossEntities() {
            List<EntityData> entityDatas = new();
            entityDatas.Add(new EntityData(
                600,
                600,
                "Boss",
                EntityTypes.ENEMY,
                EntityClasses.Boss
            ));
            return entityDatas;
        }

        public void Unlock() {
            if (!this.isVisited) this.view.Unlock();
            this.isLocked = false;
        }

        public void Resolve() {
            if (this.isLocked) this.Unlock();
            this.isVisited = true;
            this.view.Resolve();
            _UnlockChildren();
        }

        public void Lock() {
            this.isLocked = true;
            this.isVisited = false;
            this.view.Lock();
        }

        private void _UnlockChildren() {
            foreach (var node in connectedNodes) {
                node.Unlock();
            }
        }

        public void LockChildren() {
            foreach (var node in connectedNodes) {
                node.Lock();
            }
        }
    }
}
