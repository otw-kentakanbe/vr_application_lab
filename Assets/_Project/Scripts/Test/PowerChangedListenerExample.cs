using UnityEngine;
using VContainer;

// << UnityAction Process >>
public sealed class PowerChangedListener : MonoBehaviour
{
    [Inject] private AppStateHolder holder;

    private void Awake()
    {
        if (holder == null)
        {
            Debug.LogError("[PowerChangedListener] AppStateHolder is not assigned.", this);
            enabled = false;
            return;
        }

        // 登録
        holder.PowerChanged += HandlePowerChanged;
    }

    private void OnDestroy()
    {
        // 解除
        if (holder != null) holder.PowerChanged -= HandlePowerChanged;
    }

    private void HandlePowerChanged(bool isOn)
    {
        Debug.Log($"[PowerChangedListener] Power is {(isOn ? "ON" : "OFF")}.", this);
    }
}
