using Cards.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards.Handlers {
    class CardStateHandler: MonoBehaviour {
        private CardState _currentState = CardState.Idle;
        private CardState _prevState = CardState.Idle;

        void Update() {
            switch (_currentState) {
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
                default:
                    _HandleIdle();
                    break;
            }
        }

        public void SetState(CardState newState) {
            if (_currentState == newState) return;
            _ExitState(newState);
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

        }

        private void _EnterState(CardState state) {

        }

        private void _HandleIdle() { 
            /* 檢查滑鼠是否進入 → Hover */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null) return;
            if (eventHandler.IsPointerEnter()) SetState(CardState.Hover);
        }

        private void _HandleHover() { 
            /* 檢查點擊 → Active, 移開 → Idle */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null) return;
            else if (eventHandler.IsPointerExit()) SetState(CardState.Idle);
            else if (eventHandler.IsClicked()) SetState(CardState.Active);
        }

        private void _HandleActive() { 
            /* 檢查拖曳 → Dragging, 點擊 → Idle */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null) return;
            else if (eventHandler.IsClicked()) SetState(CardState.Idle);
            else if (eventHandler.IsDragging()) SetState(CardState.Dragging);
        }

        private void _HandleDragging() { 
            /* 檢查釋放 → Idle, 指向目標 → Targeting */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null) return;
            else if (!eventHandler.IsDragging()) SetState(CardState.Idle);
            else if (eventHandler.IsPointingAtTarget()) SetState(CardState.Targeting);
        }

        private void _HandleTargeting() {
            /* 離開目標 → Dragging, 釋放 → Use */ 
            CardEventHandler eventHandler = GetComponent<CardEventHandler>();
            if (eventHandler == null) return;
            else if (!eventHandler.IsPointingAtTarget()) SetState(CardState.Dragging);
            else if (!eventHandler.IsDragging()) SetState(CardState.Use);
        }

        private void _HandleUse() { 
            /* 使用卡片的瞬間，完成 → Applying */
            UseCardHandler useHandler = GetComponent<UseCardHandler>();
            if (useHandler == null) return;
            
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
            // do something
        }
    }
}
