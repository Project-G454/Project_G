namespace Cards.Data {
    public enum CardTypes {
        UNSET,
        ATTACK,
        MAGIC,
        MOVE,
        DEFENCE
    }

    public enum CardRarity {
        UNSET,
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public enum CardState {
        Idle,
        Hover,
        Active,
        Dragging,
        Targeting,
        Use,
        Applying,
        Destroy
    }
}
