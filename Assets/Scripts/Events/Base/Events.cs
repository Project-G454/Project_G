using Cards;
using Entities;

namespace Events {
    public class BattleStartEvent { }

    public class BattleEndEvent { }

    public class RoundStartEvent { public int RoundNumber; }

    public class RoundEndEvent { public int RoundNumber; }

    public class TurnStartEvent { public int entityId; }

    public class TurnEndEvent { public int entityId; }

    public class CardUseEvent { public int entityId; public int cardId; }

    public class DealDamageEvent { public int attackerId; public int victimId; }

    public class ReceiveDamageEvent { public int attackerId; public int victimId; }
}
