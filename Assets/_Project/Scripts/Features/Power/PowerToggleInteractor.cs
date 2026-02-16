using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// PowerToggleInteractor クラス
/// - タッチ入力を検知し、ToggleRequested イベントを発火する
/// - 視覚フィードバック（DOTween）は PowerToggleClickedView に委譲する
/// </summary>
public sealed class PowerToggleInteractor : MonoBehaviour, IPowerToggleInput
{
    private const string LogPrefix = "[PowerToggleInteractor]";

    public event Action ToggleRequested;
    public Transform InteractorTransform => interactorVisual;

    [Header("References")]
    [SerializeField] private XRSimpleInteractable interactable;
    [FormerlySerializedAs("buttonVisual")]
    [SerializeField] private Transform interactorVisual;

    [Header("Debounce")]
    [Tooltip("Hover Enter Spam Prevention Cooldown (seconds)")]
    [SerializeField] private float cooldownSeconds = 0.25f;

    private float _lastFireTime = -999f;

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        // transform: MonoBehaviour に用意されているプロパティ
        // 本 script がアタッチされている GameObject 自身の Transform を指す
        interactorVisual = transform;
    }

    private void Awake()
    {
        if (interactable == null)
        {
            Debug.LogError($"{LogPrefix} XRSimpleInteractable is not assigned.", this);
            enabled = false;
            return;
        }

        if (interactorVisual == null) interactorVisual = transform;

        // 「当たったら」＝ Hover Enter
        interactable.hoverEntered.AddListener(OnHoverEntered);
    }

    private void OnDestroy()
    {
        if (interactable == null) return;

        interactable.hoverEntered.RemoveListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs _)
    {
        // debounce.
        if (Time.time - _lastFireTime < cooldownSeconds) return;
        _lastFireTime = Time.time;

        // PowerStatePresenter に状態変化を伝えるイベントを発火
        ToggleRequested?.Invoke();
    }
}
