using UnityEngine;
using Core.Managers;
using UnityEngine.Rendering;

namespace Core.Utilities {
    public class SmartCharacterSorting {

        private const int ORDER_BEHIND_OBSTACLE = 4;
        private const int ORDER_IN_FRONT_OBSTACLE = 7;
        private const int ORDER_DEFAULT = 4;

        public static void SetCharacterSortingByEnvironment(GameObject character, Vector2 targetPosition) {
            if (character == null) return;

            GridManager gridManager = GridManager.Instance;
            if (gridManager == null) return;

            Vector2 upPosition = targetPosition + Vector2.up;
            Vector2 downPosition = targetPosition + Vector2.down;

            bool upBlocked = !IsPositionWalkable(gridManager, upPosition);
            bool downBlocked = !IsPositionWalkable(gridManager, downPosition);

            int targetOrder = DetermineOrderByEnvironment(upBlocked, downBlocked);

            SetCharacterOrder(character, targetOrder);
        }

        public static void SetCharacterOrder(GameObject character, int baseOrder) {
            if (character == null) return;

            // 尋找 Sorting Group（在父物件 Entity 上）
            SortingGroup sortingGroup = character.GetComponentInParent<SortingGroup>();

            if (sortingGroup != null) {
                sortingGroup.sortingLayerName = "Default";
                sortingGroup.sortingOrder = baseOrder;
            }
        }

        private static bool IsPositionWalkable(GridManager gridManager, Vector2 position) {
            return gridManager.GetTileWalkable(position);
        }

        private static int DetermineOrderByEnvironment(bool upBlocked, bool downBlocked) {
            // 上下都有 → 以下方為準
            if (upBlocked && downBlocked) {
                return ORDER_BEHIND_OBSTACLE;
            }

            // 只有下方
            if (downBlocked) {
                return ORDER_BEHIND_OBSTACLE;
            }

            // 只有上方
            if (upBlocked) {
                return ORDER_IN_FRONT_OBSTACLE;
            }

            return ORDER_DEFAULT;
        }
    }
}
