using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// PowerStateEffectsController クラス
/// - AppStatePresenter から状態変化を受けて、Bloom や Vignette などのエフェクトの強さを切り替える
/// - UnityEvent を用いて、 Inspector 上から状態変化に応じた追加処理を設定できる
/// </summary>
public sealed class PowerStateEffectsController : MonoBehaviour, IPowerStateOutput
{
    private const string LogPrefix = "[PowerStateEffectsController]";

    [Header("References")]
    [SerializeField] private Volume _globalVolume;

    [Header("PowerOn Values")]
    [SerializeField] private float _bloomOn = 1.0f;
    [SerializeField] private float _bloomOff = 0.2f;
    [SerializeField] private float _vignetteOn = 0.35f;
    [SerializeField] private float _vignetteOff = 0.10f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onPowerOn;
    [SerializeField] private UnityEvent _onPowerOff;

    private Bloom _bloom;
    private Vignette _vignette;

    private void Awake()
    {
        if (!ValidateReferences(out var profile)) return;

        BindVolumeOverrides(profile);
        InitializeOverrides();
    }

    private bool ValidateReferences(out VolumeProfile profile)
    {
        profile = null;

        if (_globalVolume == null)
        {
            Debug.LogError($"{LogPrefix} Global Volume is not assigned.", this);
            enabled = false;
            return false;
        }

        profile = _globalVolume.profile;
        if (profile != null) return true;

        Debug.LogError($"{LogPrefix} Volume profile is missing.", this);
        enabled = false;
        return false;
    }

    private void BindVolumeOverrides(VolumeProfile profile)
    {
        // set cached overrides, because accessing overrides from Volume Profile every time is costly.
        profile.TryGet(out _bloom);
        profile.TryGet(out _vignette);
    }

    private void InitializeOverrides()
    {
        if (_bloom == null) Debug.LogWarning($"{LogPrefix} Bloom override not found in Volume profile.", this);
        if (_vignette == null) Debug.LogWarning($"{LogPrefix} Vignette override not found in Volume profile.", this);
    }

    public void RenderPowerState(bool isOn)
    {
        ApplyBloom(isOn);
        ApplyVignette(isOn);

        // UnityEvent process.
        (isOn ? _onPowerOn : _onPowerOff)?.Invoke();
    }

    private void ApplyBloom(bool isOn)
    {
        if (_bloom == null) return;
        _bloom.intensity.value = isOn ? _bloomOn : _bloomOff;
    }

    private void ApplyVignette(bool isOn)
    {
        if (_vignette == null) return;
        _vignette.intensity.value = isOn ? _vignetteOn : _vignetteOff;
    }
}
