using System;
using UnityEngine;
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
    private const float DefaultCooldownSeconds = 0.25f;
    private const float DefaultLastFireTime = float.NegativeInfinity;

    public event Action ToggleRequested;
    public Transform InteractorTransform => _interactorVisual;

    [Header("References")]
    [SerializeField] private XRSimpleInteractable _interactable;
    [SerializeField] private Transform _interactorVisual;

    [Header("Debounce")]
    [Tooltip("Hover Enter Spam Prevention Cooldown (seconds)")]
    [SerializeField] private float _cooldownSeconds = DefaultCooldownSeconds;

    private float _lastFireTime = DefaultLastFireTime;

    private void Reset()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactorVisual = transform;
    }

    private void Awake()
    {
        if (!ValidateReferences()) return;

        InitializeReferences();
        BindInteractionEvents();
    }

    private void OnDestroy()
    {
        UnbindInteractionEvents();
    }

    private void OnHoverEntered(HoverEnterEventArgs _)
    {
        // debounce.
        if (Time.time - _lastFireTime < _cooldownSeconds) return;
        _lastFireTime = Time.time;

        // PowerStatePresenter に状態変化を伝えるイベントを発火
        ToggleRequested?.Invoke();
    }

    private bool ValidateReferences()
    {
        if (_interactable == null)
        {
            Debug.LogError($"{LogPrefix} XRSimpleInteractable is not assigned.", this);
            enabled = false;
            return false;
        }

        return true;
    }

    private void InitializeReferences()
    {
        if (_interactorVisual == null) _interactorVisual = transform;
    }

    private void BindInteractionEvents()
    {
        // 「当たったら」＝ Hover Enter
        _interactable.hoverEntered.AddListener(OnHoverEntered);
    }

    private void UnbindInteractionEvents()
    {
        if (_interactable == null) return;

        _interactable.hoverEntered.RemoveListener(OnHoverEntered);
    }
}
