using UnityEngine;

// AppState を生成して持つホルダー
// シーン上の参照先として使い、他コンポーネントが共通状態にアクセスできるようにする
// sealed: 「継承される前提のないコンポーネントである」ことを明示（設計意図がはっきりし、意図しない派生や挙動の拡散を防ぐ）
public sealed class AppStateHolder : MonoBehaviour
{
    public AppState State { get; } = new AppState();
}
