using UnityEngine;

/**
* AppState を保持するクラス
* - AppState をインスタンス化して保持し、他のコンポーネントが AppState にアクセスできるようにする
* - これにより、状態管理が一元化され、他のコンポーネントは AppStateHolder を介して状態にアクセスできるようになる
*/
public sealed class AppStateHolder : MonoBehaviour
{
    public AppState State { get; } = new AppState();
}
