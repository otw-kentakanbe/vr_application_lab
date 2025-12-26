using UnityEngine;

using DG.Tweening;

// XRI の XRSimpleInteractable の Hover Enter をトリガーに PowerOn をトグルする
// クールダウンで連打防止し、DOTween でボタンの押下感アニメーションを再生
public sealed class TouchToggleButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AppStateHolder holder;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    [SerializeField] private Transform buttonVisual;

    [Header("Debounce")]
    [Tooltip("Hover Enter連打を防ぐクールダウン（秒）")]
    [SerializeField] private float cooldownSeconds = 0.25f;

    private float _lastFireTime = -999f;

    private void Reset()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        buttonVisual = transform;
    }

    private void Awake()
    {
        if (holder == null)
        {
            Debug.LogError("[TouchToggleButton] AppStateHolder is not assigned.", this);
            enabled = false;
            return;
        }

        if (interactable == null)
        {
            Debug.LogError("[TouchToggleButton] XRSimpleInteractable is not assigned.", this);
            enabled = false;
            return;
        }

        if (buttonVisual == null) buttonVisual = transform;

        // 「当たったら」＝ Hover Enter
        interactable.hoverEntered.AddListener(_ =>
        {
            Debug.Log("[TouchToggleButton] hoverEntered fired!", this);

            if (Time.time - _lastFireTime < cooldownSeconds) return;
            _lastFireTime = Time.time;

            // 状態トグル
            holder.State.PowerOn.Value = !holder.State.PowerOn.Value;

            // 押した感（軽く・短く）
            buttonVisual.DOKill();
            buttonVisual.localScale = Vector3.one;
            buttonVisual.DOScale(1.6f, 0.12f)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);
        });
    }
}
