using System;
using System.Collections;
using Cards.Animations;
using Cards.Data;
using Core.Entities;
using Core.Managers;
using Core.Managers.Cards;
using Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Handlers {
    class CardStateHandler: MonoBehaviour {
        private CardState _currentState = CardState.Idle;
        private CardState _prevState = CardState.Idle;

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
            _ExitState(_currentState);
            _prevState = _currentState;
            _currentState = newState;
            _EnterState(newState);
        }

        public CardState GetState() {
            return _currentState;
        }

        public CardState GetPrevState() {
            return _prevState;
        }


        private void _ExitState(CardState state) {
            // Debug.Log($"Exit State: {state}");
        }

        private void _EnterState(CardState state) {
            // Debug.Log($"Enter State: {state}");
        }

        private void _HandleDodge() {
            Transform cardTransform = GetComponent<Transform>();
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cardTransform == null || eventHandler == null) return;

            cg.blocksRaycasts = false;
            int hovering = eventHandler.GetHoveringCardIdx();
            if (hovering != -1) CardAnimation.Dodge(cardTransform.parent, hovering, CardManager.cardList);

            if (!eventHandler.IsAnyCardHovering()) SetState(CardState.Idle);
        }

        private void _HandleIdle() { 
            /* 檢查滑鼠是否進入 → Hover */ 
            CardView view = GetComponent<CardView>();
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (eventHandler == null || view == null) return;

            StartCoroutine(_DelayClear());
            cg.blocksRaycasts = !eventHandler.IsAnyCardHovering();
            CardAnimation.ZoomOut(gameObject);
            CardAnimation.MoveTo(gameObject, view.GetInitialPosition());
            CardAnimation.ResetSibling(gameObject);
            CardAnimation.SetAlpha(gameObject, 1f);

            if (eventHandler.IsPointerEnter()) SetState(CardState.Hover);
            else if (eventHandler.IsAnyCardHovering()) SetState(CardState.Dodge);
        }

        private void _HandleHover() { 
            /* 檢查點擊 → Active, 移開 → Idle */ 
            CardView view = GetComponent<CardView>();
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            DescriptionManager descriptionManager = DescriptionManager.Instance;
            CardBehaviour cb = GetComponent<CardBehaviour>();
            if (eventHandler == null || view == null || cb == null) return;

            CardAnimation.ZoomIn(gameObject);
            CardAnimation.MoveTo(gameObject, view.GetInitialPosition() + new Vector3(0f, 20f, 0f));
            CardAnimation.SendToFront(gameObject);
            Card card = cb.card;
            
            descriptionManager.ShowOnly(card.desctiptionIds);

            if (eventHandler.IsPointerExit()) SetState(CardState.Idle);
            else if (eventHandler.IsDragging()) SetState(CardState.Dragging);
            else if (eventHandler.IsClicked()) SetState(CardState.Active);
        }

        private void _HandleActive() { 
            /* 檢查拖曳 → Dragging, 點擊 → Idle */ 
            CardView view = GetComponent<CardView>();
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null || view == null) return;

            CardAnimation.ZoomIn(gameObject);
            CardAnimation.MoveTo(gameObject, view.GetInitialPosition() + new Vector3(0f, 20f, 0f));
            CardAnimation.SendToFront(gameObject);

            if (eventHandler.IsClicked()) SetState(CardState.Idle);
            else if (eventHandler.IsDragging()) SetState(CardState.Dragging);
        }

        private void _HandleDragging() { 
            /* 檢查釋放 → Idle, 指向目標 → Targeting */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            RectTransform rt = GetComponent<RectTransform>();
            Canvas canvas = GetComponentInParent<Canvas>();
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (eventHandler == null || rt == null || canvas == null) return;
            if (_prevState != _currentState) cg.blocksRaycasts = false;
            
            PointerEventData eventData = eventHandler.GetEventData();
            if (eventData == null) return;

            CardAnimation.MoveToPointer(gameObject, eventData);
            CardAnimation.SetAlpha(gameObject, 0.5f);

            if (!eventHandler.IsDragging()) SetState(CardState.Idle);
            else if (eventHandler.IsPointingAtTarget()) SetState(CardState.Targeting);
        }

        private void _HandleTargeting() {
            /* 離開目標 → Dragging, 釋放 → Use */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            UseCardHandler useHandler = GetComponent<UseCardHandler>();
            if (useHandler == null || eventHandler == null) return;
            
            int? targetId = useHandler.GetTargetId();
            if (targetId != null) CardAnimation.MoveToEntity(gameObject, (int)targetId);

            if (!eventHandler.IsPointingAtTarget()) SetState(CardState.Dragging);
            else if (!eventHandler.IsDragging()) SetState(CardState.Use);
        }

        private void _HandleUse() { 
            /* 使用卡片的瞬間，完成 → Applying */
            UseCardHandler useHandler = GetComponent<UseCardHandler>();
            DescriptionManager descriptionManager = DescriptionManager.Instance;
            if (useHandler == null) return;
            descriptionManager.HideAll();
            
            useHandler.UseCard();
            SetState(CardState.Applying);
        }

        private void _HandleApplying() {
            /* 動畫、等待其他動作反應，完成 → Destroy */
            // do something
            SetState(CardState.Destroy);        
        }
        
        private void _HandleDestroy() {
            /* 卡片已被使用完成 */
        }

        private IEnumerator _DelayClear() {
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            DescriptionManager descriptionManager = DescriptionManager.Instance;
            yield return new WaitForSeconds(0.05f);
            if (_currentState == CardState.Idle && !eventHandler.IsAnyCardHovering()) descriptionManager.HideAll();
        }
    }
}
