using Core.Entities;
using Core.Handlers;
using Core.Managers;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnSlotUI: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Entity _entity;
    [SerializeField] private LayoutElement _layout;
    [SerializeField] private Image _avatar;
    [SerializeField] private RectTransform _redOverlay;
    [SerializeField] private GameObject _grayOverlay;
    [SerializeField] private Image _outline;
    [SerializeField] private Image _background;
    private CameraController _cameraController;

    public void OnPointerClick(PointerEventData eventData) {
        _cameraController.isFollowing = false;
        var entityObj = EntityManager.Instance.GetEntityObject(_entity.entityId);
        Vector3 targetPos = new Vector3(entityObj.transform.position.x, entityObj.transform.position.y, _cameraController.transform.position.z);
        _cameraController.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutQuart);
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        HoverUIManager.Instance.Show(_entity);
    }

    public void OnPointerExit(PointerEventData eventData) {
        HoverUIManager.Instance.Hide();
    }

    public void Init(Entity targetEntity) {
        // 解除之前註冊（保險）
        if (_entity != null)
            _entity.OnHpChanged -= UpdateRedOverlay;

        // 指定 entity
        _entity = targetEntity;

        // 綁定 avatar
        _avatar.sprite = _entity.avatar;

        // 註冊血量變化事件
        _entity.OnHpChanged += UpdateRedOverlay;

        _cameraController = Camera.main.GetComponent<CameraController>();

        // 初始化顯示
        UpdateRedOverlay();
    }

    private void OnDestroy() {
        if (_entity != null) {
            _entity.OnHpChanged -= UpdateRedOverlay;
        }
    }

    public void UpdateRedOverlay() {
        float hpPercent = (float)_entity.currentHp / _entity.maxHp;

        _redOverlay.anchorMax = new Vector2(1, 1 - hpPercent);
        _redOverlay.offsetMin = Vector2.zero;
        _redOverlay.offsetMax = Vector2.zero;
    }

    public void AnimateFocus(bool isCurrent) {
        float targetWidth = isCurrent ? 90f : 75f;
        float targetHeight = isCurrent ? 138f : 115f;
        float animationDuration = 0.5f;

        // 使用 DOTween 動畫調整尺寸
        DOTween.To(() => _layout.preferredWidth, x => _layout.preferredWidth = x, targetWidth, animationDuration);
        DOTween.To(() => _layout.preferredHeight, x => _layout.preferredHeight = x, targetHeight, animationDuration);
    }

    public void SetOutline() {
        bool isPlayer = (_entity.entityData.type == EntityTypes.PLAYER);
        _outline.color = isPlayer ? Color.cyan : Color.red;
        _background.color = isPlayer ? Color.cyan : Color.magenta;
    }

    public void UpdateGrayOverLay(bool active) {
        _grayOverlay.SetActive(active);
    }
}
