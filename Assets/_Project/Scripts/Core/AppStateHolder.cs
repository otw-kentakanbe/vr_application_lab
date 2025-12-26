using UnityEngine;

// AppState を生成して持つホルダー
// シーン上の参照先として使い、他コンポーネントが共通状態にアクセスできるようにする
public sealed class AppStateHolder : MonoBehaviour
{
    public AppState State { get; } = new AppState();
}
