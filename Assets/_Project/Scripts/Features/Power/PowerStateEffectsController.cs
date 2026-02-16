using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/**
* PowerStateEffectsController クラス
* - PowerStatePresenter から状態変化を受けて、Bloom や Vignette などのエフェクトの強さを切り替える
* - UnityEvent を用いて、状態変化に応じた追加の処理を外部から設定できるようにする
*/
public sealed class PowerStateEffectsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;

    [Header("PowerOn Values")]
    [SerializeField] private float bloomOn = 1.0f;
    [SerializeField] private float bloomOff = 0.2f;
    [SerializeField] private float vignetteOn = 0.35f;
    [SerializeField] private float vignetteOff = 0.10f;

    [Header("Events")]
    [SerializeField] private UnityEvent onPowerOn;
    [SerializeField] private UnityEvent onPowerOff;

    private Bloom _bloom;
    private Vignette _vignette;

    private void Awake()
    {
        if (globalVolume == null)
        {
            Debug.LogError("[PowerStateEffectsController] Global Volume is not assigned.", this);
            enabled = false;
            return;
        }

        if (globalVolume.profile == null)
        {
            Debug.LogError("[PowerStateEffectsController] Volume profile is missing.", this);
            enabled = false;
            return;
        }

        globalVolume.profile.TryGet(out _bloom);
        globalVolume.profile.TryGet(out _vignette);

        if (_bloom == null) Debug.LogWarning("[PowerStateEffectsController] Bloom override not found in Volume profile.", this);
        if (_vignette == null) Debug.LogWarning("[PowerStateEffectsController] Vignette override not found in Volume profile.", this);
    }

    public void ApplyPowerState(bool isOn)
    {
        // post-processing process.
        if (_bloom != null) _bloom.intensity.value = isOn ? bloomOn : bloomOff;
        if (_vignette != null) _vignette.intensity.value = isOn ? vignetteOn : vignetteOff;

        // UnityEvent process.
        (isOn ? onPowerOn : onPowerOff)?.Invoke();
    }
}
