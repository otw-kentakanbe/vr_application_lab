using UnityEngine;

public sealed class AppStateHolder : MonoBehaviour
{
    public AppState State { get; } = new AppState();
}
