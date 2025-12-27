using UnityEngine;

// << UnityAction Process >>
public sealed class PowerChangedListener : MonoBehaviour
{
    [SerializeField] private PostProcessStateBinder binder;

    private void Awake()
    {
        if (binder == null)
        {
            Debug.LogError("[PowerChangedListener] PostProcessStateBinder is not assigned.", this);
            enabled = false;
            return;
        }

        // 登録
        binder.PowerChanged += HandlePowerChanged;
    }

    private void OnDestroy()
    {
        // 解除
        if (binder != null) binder.PowerChanged -= HandlePowerChanged;
    }

    private void HandlePowerChanged(bool isOn)
    {
        Debug.Log($"[PowerChangedListener] Power is {(isOn ? "ON" : "OFF")}.", this);
    }
}
