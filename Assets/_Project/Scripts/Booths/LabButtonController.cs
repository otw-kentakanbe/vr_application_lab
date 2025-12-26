using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using R3;
using DG.Tweening;

public class LabButtonController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    [SerializeField] private Transform buttonVisual;
    [SerializeField] private Volume globalVolume;

    private readonly AppState appState = new();
    private Bloom bloom;
    private Vignette vignette;

    private void Awake()
    {
        // Volumeから参照取得
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out vignette);

        // 押したら PowerOn をトグル
        interactable.selectEntered.AddListener(_ =>
        {
            appState.PowerOn.Value = !appState.PowerOn.Value;
        });

        // 状態変化 → 演出（DOTween + Post）
        appState.PowerOn
            .Subscribe(isOn =>
            {
                // ボタン演出（酔いにくい短い動き）
                buttonVisual.DOKill();
                buttonVisual.DOPunchScale(Vector3.one * 0.08f, 0.15f, 8, 0.8f);

                // ポストエフェクト演出
                if (bloom != null) bloom.intensity.value = isOn ? 1.0f : 0.2f;
                if (vignette != null) vignette.intensity.value = isOn ? 0.35f : 0.1f;
            })
            .AddTo(this); // GameObject破棄時にDispose
    }
}
