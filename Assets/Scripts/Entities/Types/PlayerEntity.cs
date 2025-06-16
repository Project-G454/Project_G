using Core.Game;
using Core.Managers.Deck;
using Core.Managers.Energy;
using Entities.Factories;
using UnityEngine;

namespace Entities.Categories {
    public class Player: Entity {
        private GamePlayerState _state;

        public Player(
            int id,
            EntityData data
        ) : base(id, data) {
            _state = PlayerStateManager.Instance.GetPlayer(base.entityId);
            OnHpChanged += SyncHpToGameState;
        }
        
        private void SyncHpToGameState() {
            _state.currentHp = currentHp;
        }
    }
}

