using System.Collections;
using Cards.Animations;
using Cards.Data;
using Core.Managers;
using Core.Managers.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    class CardStateHandler: MonoBehaviour {
        // --- Serialize Fields ---
        [SerializeField] private float _zoomInScale = 1.2f;
        [SerializeField] private float _transformY = 140f;
        [SerializeField] private float _dodgeGap = 40f;
        
        // --- Classes ---
        private CardEventHandler _eventHandler;
        private CardView _view;
        private CanvasGroup _canvasGroup;
        private DescriptionManager _descriptionManager;
        private CardBehaviour _cardBehaviour;
        private UseCardHandler _useCardHandler;

        // --- arguments ---
        private CardState _currentState = CardState.Idle;
        private CardState _prevState = CardState.Idle;

        public void Init() {
            _view = GetComponent<CardView>();
            _eventHandler = GetComponent<CardEventHandler>();
            _cardBehaviour = GetComponent<CardBehaviour>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _useCardHandler = GetComponent<UseCardHandler>();
            _descriptionManager = DescriptionManager.Instance;
        }

        void Start() {
            Init();
        }

        void Update() {
            switch (_currentState) {
                case CardState.Dodge:
                    _HandleDodge();
                    break; 
                case CardState.Idle:
                    _HandleIdle();
                    break;
                case CardState.Hover:
                    _HandleHover();
                    break;
                case CardState.Active:
                    _HandleActive();
                    break;
                case CardState.Dragging:
                    _HandleDragging();
                    break;
                case CardState.Targeting:
                    _HandleTargeting();
                    break;
                case CardState.Use:
                    _HandleUse();
                    break;
                case CardState.Applying:
                    _HandleApplying();
                    break;
                case CardState.Destroy:
                    _HandleDestroy();
                    break;
            }
        }


        public void SetState(CardState newState) {
            if (_currentState == newState) return;
            if (_currentState != CardState.Dodge && newState != CardState.Dodge) {
                Debug.Log($"{_currentState} -> {newState}");
            }
            _prevState = _currentState;
            _currentState = newState;
        }

        public CardState GetState() {
            return _currentState;
        }

        public CardState GetPrevState() {
            return _prevState;
        }

        private void _HandleDodge() {
            _canvasGroup.blocksRaycasts = false;
            int hovering = _eventHandler.GetHoveringCardIdx();
            if (hovering != -1) {
                CardAnimation.StopAllAnimation(gameObject);
                CardAnimation.Dodge(transform.parent, hovering, CardManager.cardList, _dodgeGap);
            }

            if (!_eventHandler.IsAnyCardHovering()) SetState(CardState.Idle);
        }

        private void _HandleIdle() { 
            /* 檢查滑鼠是否進入 → Hover */
            StartCoroutine(_DelayClear());
            _canvasGroup.blocksRaycasts = !_eventHandler.IsAnyCardHovering();

            CardAnimation.StopAllAnimation(gameObject);
            CardAnimation.ZoomOut(gameObject);
            CardAnimation.MoveTo(gameObject, _view.GetInitialPosition());
            CardAnimation.ResetSibling(gameObject);
            CardAnimation.SetAlpha(gameObject, 1f);

            if (_eventHandler.IsPointerEnter()) SetState(CardState.Hover);
            else if (_eventHandler.IsAnyCardHovering()) SetState(CardState.Dodge);
        }

        private void _HandleHover() { 
            /* 檢查點擊 → Active, 移開 → Idle */
            CardAnimation.StopAllAnimation(gameObject);
            CardAnimation.ZoomIn(gameObject, _zoomInScale);
            CardAnimation.MoveTo(gameObject, _view.GetInitialPosition() + new Vector3(0f, _transformY, 0f));
            CardAnimation.SendToFront(gameObject);
            Card card = _cardBehaviour.card;
            
            _descriptionManager.ShowOnly(card.desctiptionIds);

            if (_eventHandler.IsPointerExit()) SetState(CardState.Idle);
            else if (_eventHandler.IsDragging()) SetState(CardState.Dragging);
            else if (_eventHandler.IsClicked()) SetState(CardState.Active);
        }

        private void _HandleActive() { 
            /* 檢查拖曳 → Dragging, 點擊 → Idle */
            CardAnimation.StopAllAnimation(gameObject);
            CardAnimation.ZoomIn(gameObject, _zoomInScale);
            CardAnimation.MoveTo(gameObject, _view.GetInitialPosition() + new Vector3(0f, _transformY, 0f));
            CardAnimation.SendToFront(gameObject);

            if (_eventHandler.IsClicked()) SetState(CardState.Idle);
            else if (_eventHandler.IsDragging()) SetState(CardState.Dragging);
        }

        private void _HandleDragging() { 
            /* 檢查釋放 → Idle, 指向目標 → Targeting */
            if (_prevState != _currentState) _canvasGroup.blocksRaycasts = false;
            
            PointerEventData eventData = _eventHandler.GetEventData();
            if (eventData == null) return;

            CardAnimation.StopAllAnimation(gameObject);
            CardAnimation.MoveToPointer(gameObject, eventData);
            CardAnimation.SetAlpha(gameObject, 0.5f);

            if (!_eventHandler.IsDragging()) SetState(CardState.Idle);
            else if (_eventHandler.IsPointingAtTarget()) SetState(CardState.Targeting);
        }

        private void _HandleTargeting() {
            /* 離開目標 → Dragging, 釋放 → Use */
            int? targetId = _useCardHandler.GetTargetId();
            if (targetId != null) {
                CardAnimation.StopAllAnimation(gameObject);
                CardAnimation.MoveToEntity(gameObject, (int)targetId);
            }

            if (!_eventHandler.IsPointingAtTarget()) SetState(CardState.Dragging);
            else if (!_eventHandler.IsDragging()) SetState(CardState.Use);
        }

        private void _HandleUse() { 
            /* 使用卡片的瞬間，完成 → Applying */
            _descriptionManager.HideAll();
            if (_useCardHandler.UseCard()) SetState(CardState.Applying);
            else SetState(CardState.Idle);
        }

        private void _HandleApplying() {
            /* 動畫、等待其他動作反應，完成 → Destroy */
            SetState(CardState.Destroy);
        }
        
        private void _HandleDestroy() {
            /* 卡片已被使用完成 */
            CardManager.Instance.RemoveCard(gameObject);
            CardManager.Instance.SetNewCardPosition();
            Destroy(gameObject);
        }

        private IEnumerator _DelayClear() {
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            DescriptionManager descriptionManager = DescriptionManager.Instance;
            yield return new WaitForSeconds(0.05f);
            if (_currentState == CardState.Idle && !eventHandler.IsAnyCardHovering()) descriptionManager.HideAll();
        }
    }
}
