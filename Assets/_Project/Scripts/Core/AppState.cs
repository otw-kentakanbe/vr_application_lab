using R3;

// アプリ全体の状態モデル
// PowerOn を ReactiveProperty<bool> として保持し、他コンポーネントが購読できるようにする
public sealed class AppState
{
    // 例：触れたらON/OFFを切り替える状態
    public ReactiveProperty<bool> PowerOn { get; } = new(false);
}
