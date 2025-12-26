using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using R3;

// PowerOn の状態に応じて Volume の Bloom/Vignette 強度を切り替えるバインダー
// 未割当の参照は警告/エラーで停止
public sealed class PostProcessStateBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AppStateHolder holder;
    [SerializeField] private Volume globalVolume;

    [Header("PowerOn Values")]
    [SerializeField] private float bloomOn = 1.0f;
    [SerializeField] private float bloomOff = 0.2f;
    [SerializeField] private float vignetteOn = 0.35f;
    [SerializeField] private float vignetteOff = 0.10f;

    private Bloom _bloom;
    private Vignette _vignette;

    private void Awake()
    {
        if (holder == null)
        {
            Debug.LogError("[PostProcessStateBinder] AppStateHolder is not assigned.", this);
            enabled = false;
            return;
        }

        if (globalVolume == null)
        {
            Debug.LogError("[PostProcessStateBinder] Global Volume is not assigned.", this);
            enabled = false;
            return;
        }

        if (globalVolume.profile == null)
        {
            Debug.LogError("[PostProcessStateBinder] Volume profile is missing.", this);
            enabled = false;
            return;
        }

        globalVolume.profile.TryGet(out _bloom);
        globalVolume.profile.TryGet(out _vignette);

        if (_bloom == null) Debug.LogWarning("[PostProcessStateBinder] Bloom override not found in Volume profile.", this);
        if (_vignette == null) Debug.LogWarning("[PostProcessStateBinder] Vignette override not found in Volume profile.", this);

        holder.State.PowerOn
            .Subscribe(isOn =>
            {
                if (_bloom != null) _bloom.intensity.value = isOn ? bloomOn : bloomOff;
                if (_vignette != null) _vignette.intensity.value = isOn ? vignetteOn : vignetteOff;
            })
            .AddTo(this);
    }
}
