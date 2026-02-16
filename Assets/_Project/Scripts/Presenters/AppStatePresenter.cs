using R3;
using UnityEngine;
using VContainer;

/**
* AppStateHolder から AppState を取得、操作（トグルボタン）に応じて状態を更新、状態変化を伝えるクラス
* - 状態管理と UI/エフェクトの更新を連携
*/
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
        // イベントの購読解除を行わないと、オブジェクトが破棄された後もイベントが発火し、NullReferenceException が発生する可能性がある
        if (_toggleView != null) _toggleView.ToggleRequested -= OnToggleRequested;
    }

    private void OnToggleRequested()
    {
        _holder.State.PowerOn.Value = !_holder.State.PowerOn.Value;
    }
}
