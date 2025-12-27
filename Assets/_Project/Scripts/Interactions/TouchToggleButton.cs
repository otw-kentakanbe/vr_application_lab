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
            Debug.Log("[TouchToggleButton] hoverEntered fired!", this);

            if (Time.time - _lastFireTime < cooldownSeconds) return;
            _lastFireTime = Time.time;

            // << R3 Process >>
            // 状態トグル
            // ReactiveProperty<bool> が値変更を検知して購読者に通知する
            // このプロジェクトでは PostProcessStateBinder が PowerOn を Subscribe しているので、同フレーム内で isOn が流れ、Bloom/Vignette の強度が即座に更新される
            // isOn: 引数として渡される（（Subscribe は基本的に1つの引数のみ受け付ける）
            // AddTo(this) により、バインダーが破棄されると購読も自動解除される
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
