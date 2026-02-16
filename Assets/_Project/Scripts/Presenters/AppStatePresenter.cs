using R3;
using UnityEngine;
using VContainer;

// MVP: Presenter that connects input view and post-process view with AppState (model).
public sealed class AppStatePresenter : MonoBehaviour
{
    [Inject] private AppStateHolder _holder;
    [Inject] private TouchToggleButton _toggleView;
    [Inject] private PostProcessStateBinder _postProcessView;

    private void Start()
    {
        if (_holder == null || _toggleView == null || _postProcessView == null)
        {
            Debug.LogError("[AppStatePresenter] Dependencies are not injected.", this);
            enabled = false;
            return;
        }

        _toggleView.ToggleRequested += OnToggleRequested;

        _holder.State.PowerOn
            .Subscribe(isOn => _postProcessView.ApplyPowerState(isOn))
            .AddTo(this);

        _postProcessView.ApplyPowerState(_holder.State.PowerOn.Value);
    }

    private void OnDestroy()
    {
        if (_toggleView != null) _toggleView.ToggleRequested -= OnToggleRequested;
    }

    private void OnToggleRequested()
    {
        _holder.State.PowerOn.Value = !_holder.State.PowerOn.Value;
    }
}
