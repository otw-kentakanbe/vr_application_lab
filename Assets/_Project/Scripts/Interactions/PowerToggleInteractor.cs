using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/**
* PowerToggleInteractor クラス
* - タッチを検知し、下記の処理を行う
*.  - AppStatePresenter から ToggleRequested イベントを受け取って状態を更新
*.  - Animation(DoTween)を再生
*/
public sealed class PowerToggleInteractor : MonoBehaviour
{
    public event Action ToggleRequested;

    [Header("References")]
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private Transform buttonVisual;

    [Header("Debounce")]
    [Tooltip("Hover Enter Spam Prevention Cooldown (seconds)")]
    [SerializeField] private float cooldownSeconds = 0.25f;

    private float _lastFireTime = -999f;

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        // transform: MonoBehaviour に用意されているプロパティ
        // 本 script がアタッチされている GameObject 自身の Transform を指す
        buttonVisual = transform;
    }

    private void Awake()
    {
        if (interactable == null)
        {
            Debug.LogError("[PowerToggleInteractor] XRSimpleInteractable is not assigned.", this);
            enabled = false;
            return;
        }

        if (buttonVisual == null) buttonVisual = transform;

        // 「当たったら」＝ Hover Enter
        interactable.hoverEntered.AddListener(OnHoverEntered);
    }

    private void OnDestroy()
    {
        if (interactable != null) interactable.hoverEntered.RemoveListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs _)
    {
        // debounce.
        if (Time.time - _lastFireTime < cooldownSeconds) return;
        _lastFireTime = Time.time;

        // AppStatePresenter に状態変化を伝えるイベントを発火
        ToggleRequested?.Invoke();

        // DoTween.
        // dokill: 既に再生中の同一プロパティのアニメーションがあれば、それを停止してから新しいアニメーションを開始する
        // dojump: 対象 Transform を指定した位置にジャンプさせるアニメーションを再生する
        buttonVisual.DOKill();
        buttonVisual.DOJump(new Vector3(4, 0, 0), 0.5f, 1, 3.0f);
    }
}
