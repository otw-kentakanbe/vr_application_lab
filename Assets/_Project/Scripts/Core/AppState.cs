using R3;

public sealed class AppState
{
    // 例：触れたらON/OFFを切り替える状態
    public ReactiveProperty<bool> PowerOn { get; } = new(false);
}
