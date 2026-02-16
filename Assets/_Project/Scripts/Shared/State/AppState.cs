using R3;

/// <summary>
/// App 全体の状態を管理するクラス(状態管理を一元化)
/// - ReactiveProperty として持ち、他のコンポーネントが監視して状態変化に応じた処理を行えるようにする
///   - AppStateHolder:状態の保持
///   - PowerStatePresenter:状態の更新と他コンポーネントへの通知
/// </summary>
public sealed class AppState
{
    public ReactiveProperty<bool> ReactivePowerOn { get; } = new(false);
}
