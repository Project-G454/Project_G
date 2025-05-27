using System.Collections.Generic;
using Core.Helpers;
using Core.Interfaces;
using Core.Managers;
using WorldMap.Models;

public static class EntryManagerDispatcher {
    private static readonly Dictionary<NodeType, IEntryManager> managerMap = new() {
        { NodeType.Battle, new BattleManager() },
    };

    public static void Enter(NodeType type) {
        if (managerMap.TryGetValue(type, out var manager)) {
            SceneTransitionHelper.LoadBattleScene();
        }
    }
}
