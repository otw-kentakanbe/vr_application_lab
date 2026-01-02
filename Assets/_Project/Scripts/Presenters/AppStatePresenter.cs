using R3;
using UnityEngine;

// MVP: Presenter that connects input view and post-process view with AppState (model).
public sealed class AppStatePresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private AppStateHolder holder;

    [Header("Views")]
    [SerializeField] private TouchToggleButton toggleView;
    [SerializeField] private PostProcessStateBinder postProcessView;

    private void Start()
    {
        if (holder == null || toggleView == null || postProcessView == null)
        {
            Debug.LogError("[AppStatePresenter] References are not assigned.", this);
            enabled = false;
            return;
        }

        toggleView.ToggleRequested += OnToggleRequested;

        holder.State.PowerOn
            .Subscribe(isOn => postProcessView.ApplyPowerState(isOn))
            .AddTo(this);

        postProcessView.ApplyPowerState(holder.State.PowerOn.Value);
    }

    private void OnDestroy()
    {
        if (toggleView != null) toggleView.ToggleRequested -= OnToggleRequested;
    }

    private void OnToggleRequested()
    {
        holder.State.PowerOn.Value = !holder.State.PowerOn.Value;
    }
}
