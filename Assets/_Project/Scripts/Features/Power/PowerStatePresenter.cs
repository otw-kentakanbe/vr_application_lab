using R3;
using UnityEngine;
using VContainer;

/**
* PowerStatePresenter クラス
* - AppStateHolder から状態を取得し、IPowerStateOutput を通じて状態変化を通知する
* - IPowerToggleInput のイベントを購読して、状態の切り替えを行う
* - IPowerStateOutput の RenderPowerState を呼び出して、UI やエフェクトの更新を行う
*/
public sealed class PowerStatePresenter : MonoBehaviour
{
    private const string LogPrefix = "[PowerStatePresenter]";

    [Inject] private AppStateHolder _holder;
    [Inject] private IPowerToggleInput _powerToggleInput;
    [Inject] private IPowerStateOutput _powerStateOutput;
    [Inject] private PowerToggleClickedView _powerToggleClickedView;

    private void Start()
    {
        if (_holder == null || _powerToggleInput == null || _powerStateOutput == null || _powerToggleClickedView == null)
        {
            Debug.LogError($"{LogPrefix} Dependencies are not injected.", this);
            enabled = false;
            return;
        }

        _powerToggleInput.ToggleRequested += OnToggleRequested;
        _holder.State.ReactivePowerOn
            .Subscribe(isOn => _powerStateOutput.RenderPowerState(isOn))
            .AddTo(this);
        _powerStateOutput.RenderPowerState(_holder.State.ReactivePowerOn.Value);
    }

    private void OnDestroy()
    {
        // イベントの購読解除を行わないと、オブジェクトが破棄された後もイベントが発火し、NullReferenceException が発生する可能性がある
        if (_powerToggleInput != null) _powerToggleInput.ToggleRequested -= OnToggleRequested;
    }

    private void OnToggleRequested()
    {
        // play the toggle click effect.(DoTween)
        _powerToggleClickedView.Play();
        // swith the power state,
        _holder.State.ReactivePowerOn.Value = !_holder.State.ReactivePowerOn.Value;
    }
}
