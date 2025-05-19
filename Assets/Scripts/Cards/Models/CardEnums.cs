namespace Cards.Data {
    public enum CardTypes {
        UNSET,
        ATTACK,
        MAGIC,
        ENERGY,
        DEFENCE,
        HEAL
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
        Dodge,
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
