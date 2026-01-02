using UnityEngine;
using R3;
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

        holder.State.PowerOn
            .Subscribe(HandlePowerChanged)
            .AddTo(this);
    }

    private void HandlePowerChanged(bool isOn)
    {
        Debug.Log($"[PowerChangedListener] Power is {(isOn ? "ON" : "OFF")}.", this);
    }
}
