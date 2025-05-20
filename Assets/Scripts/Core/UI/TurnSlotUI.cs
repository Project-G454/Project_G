using Core.Managers;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnSlotUI: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Entity entity;
    public LayoutElement layout;
    public RectTransform redOverlay;
    public GameObject grayOverlay;

    public void OnPointerEnter(PointerEventData eventData) {
        HoverUIManager.Instance.Show(entity);
    }

    public void OnPointerExit(PointerEventData eventData) {
        HoverUIManager.Instance.Hide();
    }

    public void Init(Entity targetEntity) {
        // 解除之前註冊（保險）
        if (entity != null)
            entity.OnHpChanged -= UpdateRedOverlay;

        // 指定 entity 並初始化顯示
        entity = targetEntity;

        // 註冊血量變化事件
        entity.OnHpChanged += UpdateRedOverlay;

        // 初始化顯示
        UpdateRedOverlay();
    }

    private void OnDestroy() {
        if (entity != null) {
            entity.OnHpChanged -= UpdateRedOverlay;
        }
    }

    public void UpdateRedOverlay() {

        Debug.Log("UpdateRedOverlay");
        float hpPercent = (float)entity.currentHp / entity.maxHp;
        Debug.Log($"hpPercent: {hpPercent}");

        redOverlay.anchorMax = new Vector2(1, 1 - hpPercent);
        redOverlay.offsetMin = Vector2.zero;
        redOverlay.offsetMax = Vector2.zero;
        Debug.Log($"RedOverlay anchor after: {redOverlay.anchorMax.y}");
    }

    public void AnimateFocus(bool isCurrent) {
        float targetWidth = isCurrent ? 90f : 75f;
        float targetHeight = isCurrent ? 138f : 115f;
        float animationDuration = 0.5f;

        // 使用 DOTween 動畫調整尺寸
        DOTween.To(() => layout.preferredWidth, x => layout.preferredWidth = x, targetWidth, animationDuration);
        DOTween.To(() => layout.preferredHeight, x => layout.preferredHeight = x, targetHeight, animationDuration);
    }

    public void SetOutline() {
        var transform = gameObject.transform;
        Image outline = transform.Find("Outline").GetComponent<Image>();
        Image background = transform.Find("Background").GetComponent<Image>();
        outline.color = (entity.entityData.type == EntityTypes.PLAYER) ? Color.cyan : Color.red;
        background.color = (entity.entityData.type == EntityTypes.PLAYER) ? Color.cyan : Color.magenta;
    }

    public void UpdateGrayOverLay(bool active) {
        grayOverlay.SetActive(active);
    }
}
