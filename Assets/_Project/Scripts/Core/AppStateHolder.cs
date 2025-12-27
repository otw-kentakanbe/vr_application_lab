using UnityEngine;
using UnityEngine.Events;
using R3;

// AppState を生成して持つホルダー
// シーン上の参照先として使い、他コンポーネントが共通状態にアクセスできるようにする
// sealed: 「継承される前提のないコンポーネントである」ことを明示（設計意図がはっきりし、意図しない派生や挙動の拡散を防ぐ）
public sealed class AppStateHolder : MonoBehaviour
{
    public AppState State { get; } = new AppState();
    public event UnityAction<bool> PowerChanged;

    private void Awake()
    {
        // ReactiveProperty<bool> が値変更を検知して購読者に通知する
        // isOn: 引数として渡される（Subscribe は基本的に1つの引数のみ受け付ける）
        // AddTo(this) により、ホルダーが破棄されると購読も自動解除される
        State.PowerOn
            .Subscribe(isOn => PowerChanged?.Invoke(isOn))
            .AddTo(this);
    }
}
