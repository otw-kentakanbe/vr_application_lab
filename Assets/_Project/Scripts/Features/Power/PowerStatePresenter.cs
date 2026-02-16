using System;
using R3;
using UnityEngine;
using VContainer;

/// <summary>
/// PowerStatePresenter クラス
/// - AppStateHolder から状態を取得し、IPowerStateOutput を通じて状態変化を通知する
/// - IPowerToggleInput のイベントを購読して、状態の切り替えを行う
/// - IPowerStateOutput の RenderPowerState を呼び出して、UI やエフェクトの更新を行う
/// </summary>
public sealed class PowerStatePresenter : MonoBehaviour
{
    private const string LogPrefix = "[PowerStatePresenter]";

    [Inject] private AppStateHolder _holder;
    [Inject] private IPowerToggleInput _powerToggleInput;
    [Inject] private IPowerStateOutput _powerStateOutput;
    [Inject] private PowerToggleClickedView _powerToggleClickedView;
    private bool _isInitialized;
    private bool _isInputBound;
    private IDisposable _powerStateSubscription;

    private void Start()
    {
        if (!ValidateDependencies()) return;

        _isInitialized = true;
        BindState();
        BindInputs();
    }

    private void OnEnable()
    {
        // when starting, _isInitialized is false, so skip binding/initialize until Start runs.
        if (!_isInitialized) return;

        BindState();
        BindInputs();
    }

    private void OnDisable()
    {
        UnbindInputs();
        UnbindState();
    }

    private void OnDestroy()
    {
        UnbindInputs();
        UnbindState();
    }

    private void OnToggleRequested()
    {
        // play the toggle click effect.(DoTween)
        _powerToggleClickedView.Play();
        // switch the power state,
        _holder.State.ReactivePowerOn.Value = !_holder.State.ReactivePowerOn.Value;
    }

    private bool ValidateDependencies()
    {
        if (_holder == null || _powerToggleInput == null || _powerStateOutput == null || _powerToggleClickedView == null)
        {
            Debug.LogError($"{LogPrefix} Dependencies are not injected.", this);
            enabled = false;
            return false;
        }

        return true;
    }

    private void BindInputs()
    {
        // avoid multiple subscription.
        if (_isInputBound || _powerToggleInput == null) return;

        _powerToggleInput.ToggleRequested += OnToggleRequested;
        _isInputBound = true;
    }

    private void BindState()
    {
        // avoid multiple subscription.
        if (_powerStateSubscription != null) return;

        _powerStateSubscription = _holder.State.ReactivePowerOn
            .Subscribe(isOn => _powerStateOutput.RenderPowerState(isOn));
    }

    private void UnbindInputs()
    {
        // イベントの購読解除を行わないと、オブジェクトが破棄された後もイベントが発火し、NullReferenceException が発生する可能性がある
        if (!_isInputBound || _powerToggleInput == null) return;

        _powerToggleInput.ToggleRequested -= OnToggleRequested;
        _isInputBound = false;
    }

    private void UnbindState()
    {
        _powerStateSubscription?.Dispose();
        _powerStateSubscription = null;
    }
}
