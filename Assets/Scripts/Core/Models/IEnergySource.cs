namespace Core.Interfaces {
    /// <summary>
    /// 提供能量加成的來源（如 Buff、遺物、角色被動），用於回合開始計算能量加值。
    /// </summary>
    public interface IEnergySource {
        /// <summary>
        /// 回合開始時額外提供的能量（正值、負值皆可）。
        /// </summary>
        int GetEnergyModifier();

        /// <summary>
        /// 能量來源的名稱，用於 UI 顯示。
        /// </summary>
        string GetSourceName();
    }
}
