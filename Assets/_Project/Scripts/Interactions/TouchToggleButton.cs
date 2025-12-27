using UnityEngine;
using DG.Tweening;
using VContainer;

// XRI の XRSimpleInteractable の Hover Enter をトリガーに PowerOn をトグルする
// クールダウンで連打防止し、DOTween でボタンの押下感アニメーションを再生
public sealed class TouchToggleButton : MonoBehaviour
{
    [Inject] private AppStateHolder holder;

    [Header("References")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    [SerializeField] private Transform buttonVisual;

    [Header("Debounce")]
    [Tooltip("Hover Enter Spam Prevention Cooldown (seconds)")]
    [SerializeField] private float cooldownSeconds = 0.25f;

    private float _lastFireTime = -999f;

    private void Reset()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        // transform: MonoBehaviour に用意されているプロパティ
        // 本 script がアタッチされている GameObject 自身の Transform を指す
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
            if (Time.time - _lastFireTime < cooldownSeconds) return;
            _lastFireTime = Time.time;

            // << R3 Process >>
            // 状態トグル（AppStateHolder が PowerChanged を発火）
            holder.State.PowerOn.Value = !holder.State.PowerOn.Value;

            // << DoTween >>
            // 押した感（軽く・短く）
            buttonVisual.DOKill();
            buttonVisual.localScale = Vector3.one;
            buttonVisual.DOScale(1.6f, 0.12f)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);

            // memo: Jump
            // buttonVisual.DOJump(new Vector3(5, 0, 0), 3.0f, 2, 2.0f);
            
            // memo: Circle rotation,
            // buttonVisual.DOLocalPath(
            //     new[]
            //     {
            //         new Vector3(4f, -1.2f, 0f),
            //         new Vector3(10f, 0f, 0f),
            //         new Vector3(5, 1.5f, 0),
            //     },
            //     3f, PathType.CatmullRom
            // );
        });
    }
}
