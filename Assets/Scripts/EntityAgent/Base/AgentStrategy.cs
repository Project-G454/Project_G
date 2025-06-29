using System.Collections.Generic;
using Agents.Helpers;
using Core.Helpers;
using Cards;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using UnityEngine;
using Entities.Handlers;

namespace Agents {
    public abstract class AgentStrategy {
        protected EntityAgent _agent;
        public bool isCardAnimationEnd = true;

        public virtual bool Execute(EntityAgent agent) {
            _agent = agent;
            return true;
        }

        protected Entity _FindNearestPlayer() {
            // 找出距離最近的玩家
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            Entity target = null;
            float minDistance = float.MaxValue;
            foreach (Entity entity in players) {
                float distance = DistanceHelper.ManhattanDistance(_agent.entity.position, entity.position);
                if (distance < minDistance && !entity.IsDead()) {
                    minDistance = distance;
                    target = entity;
                }
            }
            return target;
        }

        protected void _UseCard(CardBehaviour cardBehaviour, int targetId) {
            if (CardManager.Instance.UseCard(cardBehaviour, targetId, () => { isCardAnimationEnd = true; })) {
                cardBehaviour.DestroySelf();
            }
        }

        protected Vector2 _FindBestPosition(Vector2 targetPos, int maxDistance) {
            Vector2 agentPos = _agent.transform.position;

            Queue<Vector2> q = new();
            HashSet<Vector2> visited = new();

            // 初始化
            q.Enqueue(agentPos);
            visited.Add(agentPos);

            Vector2 res = agentPos;
            float resDistance = float.MaxValue;

            while (q.Count > 0) {
                Vector2 curr = q.Dequeue();

                // 超出地圖範圍 -> 跳過
                if (!IsTile(curr)) {
                    Debug.LogWarning($"{curr} is not tile");
                    continue;
                }

                // 在可移動範圍內，且不為障礙物 -> 可視為目的地
                if (DistanceHelper.InRange(curr, agentPos, maxDistance) && _IsWalkable(curr)) {
                    DebugDrawPoint(curr, Color.green, 0.1f, 1f);
                    float currDistance = DistanceHelper.ManhattanDistance(targetPos, curr);

                    if (currDistance < resDistance && DistanceHelper.ManhattanDistance(agentPos, curr) <= maxDistance) {
                        res = curr;
                        resDistance = currDistance;

                        // 如果就是目標位置，直接結束
                        if (curr == targetPos) break;
                    }
                }

                // 搜尋四個方向
                Vector2[] directions = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
                foreach (Vector2 dir in directions) {
                    Vector2 next = curr + dir;
                    if (!visited.Contains(next)) {
                        visited.Add(next);
                        q.Enqueue(next);
                    }
                }
            }

            return res;
        }

        protected bool _IsWalkable(Vector2 pos) {
            return GridManager.Instance.GetTileWalkable(pos) && !_PlayerAt(pos);
        }

        protected bool _PlayerAt(Vector2 pos) {
            List<Entity> players = EntityManager.Instance.GetEntitiesByType(EntityTypes.PLAYER);
            foreach (Entity player in players) {
                if (player.position == pos) return true;
            }
            return false;
        }

        protected bool IsTile(Vector2 pos) {
            return GridManager.Instance.GetTileAtPosition(pos) != null;
        }

        protected void DebugDrawPoint(Vector3 pos, Color color, float size = 0.1f, float duration = 0.1f) {
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;

            Debug.DrawLine(pos - up, pos + up, color, duration);
            Debug.DrawLine(pos - right, pos + right, color, duration);
        }

        protected bool CanMoveTo(Vector2 to) {
            GameObject currObj = EntityManager.Instance.GetEntityObject(_agent.entity.entityId);
            MoveHandler moveHandler = currObj.GetComponent<MoveHandler>();
            return moveHandler.CanReachPosition(to);
        }
    }
}
